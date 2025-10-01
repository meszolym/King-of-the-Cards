from rx import operators

from ImageProcessing.Preprocessor import Preprocessor
from Models.RoisContainer import RoisContainer
from ImageProcessing.MessageProcessor import *
from ImageProcessing.CardProcessor import *
from Models.Enums import Message
from Models.Card import Card
from Models.Table import Table
from CardCounting.Organizer import organize_players_cards, organize_dealer_cards
from rx.scheduler import ThreadPoolScheduler
import multiprocessing

class ProcessConductor:

    preprocessor: Preprocessor
    table_state: Table
    msg_processor: MessageProcessor
    pool_scheduler : ThreadPoolScheduler

    def __init__(self):
        self.table_state = Table(None, None, None)
        self.preprocessor = None
        self.pool_scheduler = ThreadPoolScheduler(multiprocessing.cpu_count())
        self.msg_processor = MessageProcessor()
        return

    def read_possible_messages(self, filepath: str):
        if self.msg_processor is None:
            self.msg_processor = MessageProcessor()
        self.msg_processor.read_possible_messages(filepath)
        return

    def rois_selected_handler(self, rois : RoisContainer):
        if self.preprocessor is not None:
            self.preprocessor.rois = rois
            return

        self.preprocessor = Preprocessor(rois)
        self.preprocessor.message_image_observable.pipe(
            operators.observe_on(self.pool_scheduler)
        ).subscribe(lambda img: self.message_image_handler(img))


        self.preprocessor.dealer_image_observable.subscribe(lambda img: self.dealer_image_handler(img))
        self.preprocessor.player_image_observable.subscribe(lambda img: self.player_image_handler(img))
        return

    def start_preprocessor(self):
        self.preprocessor.start()
        return

    def stop_preprocessor(self):
        self.preprocessor.stop()
        return

    def message_image_handler(self, image):
        msg : Message = self.msg_processor.process_message(image)
        match msg:
            case Message.Shuffling:
                print("Shuffling detected") #TODO: reset stuff
            case Message.WaitingForBets:
                print("Waiting for bets detected")
            #...
        # TODO: Update GUI
        return

    def dealer_image_handler(self, image):
        cards : list[Card] = process_cards(image, 1)
        #TODO: organizer call to update dealer hand
        #TODO: Update GUI
        return

    def player_image_handler(self, image):
        #cards : list[Card] = process_cards(image)
        #organize_players_cards(cards, self.table_state)
        #TODO: Update GUI
        return