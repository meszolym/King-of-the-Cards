import json
import multiprocessing
import threading

import cv2 as cv
from rx import operators
from rx.scheduler import ThreadPoolScheduler
from rx.subject import Subject

from CardCounting.BasicStrategyLogic import read_strategy
from CardCounting.Organizer import organize_dealer_cards, organize_players_cards
from ImageProcessing.CardProcessor import CardProcessor
from ImageProcessing.MessageProcessor import MessageProcessor

from ImageProcessing.Utils import take_screenshot, get_roi
from Models.BasicStrategy import BasicStrategy
from Models.BoundingBox import BoundingBox
from Models.Card import Card
from Models.CardSizesContainer import CardSizesContainer
from Models.Enums import CardType, Message
from Models.JsonDataContainer import JsonDataContainer
from Models.OverlayModel import overlay_data_from_table
from Models.RoisContainer import RoisContainer
from Models.Table import Table


class ProcessConductor:

    card_processor: CardProcessor
    msg_processor: MessageProcessor

    rois: RoisContainer

    table_state: Table
    basic_strategy : BasicStrategy

    pool_scheduler : ThreadPoolScheduler

    done_reading_rois_and_card_dimensions_json_observable : Subject
    overlay_data_update_observable : Subject

    def __init__(self):
        self.running_image_processing = False
        self.table_state = Table(None, [], [])
        self.basic_strategy = BasicStrategy([],0,0)
        self.rois = None
        self.card_processor = CardProcessor()
        self.pool_scheduler = ThreadPoolScheduler(multiprocessing.cpu_count())
        self.msg_processor = MessageProcessor()

        self.done_reading_rois_and_card_dimensions_json_observable = Subject()
        self.overlay_data_update_observable = Subject()
        return

    def read_possible_messages(self, filepath: str):
        if self.msg_processor is None:
            self.msg_processor = MessageProcessor()
        self.msg_processor.read_possible_messages(filepath)
        return

    def read_basic_strategy(self, filepath: str):
        self.basic_strategy = read_strategy(filepath)
        return

    def read_rois_and_card_dimensions(self, filepath: str):
        with open(filepath, "r") as f:
            data = json.load(f)
        rois = data.get("Rois", {})
        sizes = data.get("Sizes", {})

        dealer_roi_raw = rois.get("DealerRoi", [0.0, 0.0, 0.0, 0.0])
        dealer_roi = BoundingBox(dealer_roi_raw[0], dealer_roi_raw[1], dealer_roi_raw[2], dealer_roi_raw[3])
        player_roi_raw = rois.get("PlayerRoi", [0.0, 0.0, 0.0, 0.0])
        player_roi = BoundingBox(player_roi_raw[0], player_roi_raw[1], player_roi_raw[2], player_roi_raw[3])
        message_roi_raw = rois.get("MessageRoi", [0.0, 0.0, 0.0, 0.0])
        message_roi = BoundingBox(message_roi_raw[0], message_roi_raw[1], message_roi_raw[2], message_roi_raw[3])
        base_image_path = rois.get("BaseImagePath", "")

        dealer_card_size = sizes.get("DealerCardSize", 0.0)
        player_card_size = sizes.get("PlayerCardSize", 0.0)
        img = cv.imread(base_image_path)

        rois_container = RoisContainer(dealer_roi, player_roi, message_roi, img)
        sizes_container = CardSizesContainer(dealer_card_size, player_card_size)

        self.rois_selected_handler(rois_container)
        self.card_sizes_selected_handler(sizes_container)

        self.done_reading_rois_and_card_dimensions_json_observable.on_next(JsonDataContainer(rois_container, sizes_container))
        return

    def rois_selected_handler(self, rois : RoisContainer):
        self.rois = rois
        return

    def card_sizes_selected_handler(self, sizes : CardSizesContainer):
        self.card_processor.card_sizes = sizes
        pass

    def start_detection(self):
        self.running_image_processing = True
        detection = threading.Thread(target=self.detection_loop)
        detection.start()
        return

    def detection_loop(self):
        if self.rois is None:
            raise (Exception("ROIs not set"))

        print("Detection started")

        while self.running_image_processing:
            img = take_screenshot()
            print("Screenshot taken")
            self.message_image_handler(get_roi(img.copy(), self.rois.message_roi))
            self.dealer_image_handler(get_roi(img.copy(), self.rois.dealer_roi))
            self.player_image_handler(get_roi(img.copy(), self.rois.player_roi))
            print("Rois passed")
            #TODO: Non-blocking wait
        return

    def stop_detection(self):
        self.running_image_processing = False
        return

    def message_image_handler(self, image):
        # msg : Message = self.msg_processor.process_message(image)
        # match msg:
        #     case Message.Shuffling:
        #         print("Shuffling detected") #TODO: reset stuff
        #     case Message.WaitingForBets:
        #         print("Waiting for bets detected")
        #     #...
        # self.overlay_data_update_observable.on_next(overlay_data_from_table(self.table_state, self.basic_strategy))
        return

    def dealer_image_handler(self, image):
        cards : list[Card] = self.card_processor.process_cards(image, CardType.Dealer)
        if len(cards) == 0:
            return
        roi = self.rois.dealer_roi
        organize_dealer_cards(cards, self.table_state, roi.x, roi.y)
        self.overlay_data_update_observable.on_next(overlay_data_from_table(self.table_state, self.basic_strategy))
        return

    def player_image_handler(self, image):
        cards : list[Card] = self.card_processor.process_cards(image, CardType.Player)
        if len(cards) == 0:
            return
        roi = self.rois.player_roi
        organize_players_cards(cards, self.table_state, roi.x, roi.y)
        self.overlay_data_update_observable.on_next(overlay_data_from_table(self.table_state, self.basic_strategy))
        return

    def write_rois_and_card_dimensions(self, filepath):
        if self.rois is None or self.card_processor is None:
            return
        rois = self.rois
        sizes = self.card_processor.card_sizes
        image_path = filepath[:-5] + ".jpg"


        data = {
            "Rois": {
                "DealerRoi": [rois.dealer_roi.x, rois.dealer_roi.y, rois.dealer_roi.w, rois.dealer_roi.h],
                "PlayerRoi": [rois.player_roi.x, rois.player_roi.y, rois.player_roi.w, rois.player_roi.h],
                "MessageRoi": [rois.message_roi.x, rois.message_roi.y, rois.message_roi.w, rois.message_roi.h],
                "BaseImagePath": image_path
            },
            "Sizes": {
                "DealerCardSize": sizes.dealer_card,
                "PlayerCardSize": sizes.player_card
            },
        }
        with open(filepath, "w") as f:
            json.dump(data, f, indent=4)

        cv.imwrite(image_path, rois.base_image)

        return