from ImageProcessing.Preprocessor import Preprocessor
from Models.RoisContainer import RoisContainer
from ImageProcessing.MessageProcessor import *
from ImageProcessing.CardProcessor import *
from Models.Enums import Message
from Models.Card import Card
from Models.Table import Table
from CardCounting.Organizer import organize_players_cards, organize_dealer_cards
class ProcessConductor:

    preprocessor: Preprocessor
    table_state: Table

    def __init__(self):
        self.table_state = Table()
        return

    def rois_selected_handler(self, rois : RoisContainer):
        self.preprocessor = Preprocessor(rois)

        self.preprocessor.message_image_observable.subscribe(lambda img: self.message_image_handler(img))
        self.preprocessor.dealer_image_observable.subscribe(lambda img: self.dealer_image_handler(img))
        self.preprocessor.player_image_observable.subscribe(lambda img: self.player_image_handler(img))
        return

    def start_preprocessor(self):
        self.preprocessor.mainloop()
        return

    def stop_preprocessor(self):
        self.preprocessor.stop()
        return

    @staticmethod
    def message_image_handler(self, image):
        msg : Message = process_message(image)
        match msg:
            case Message.Shuffling:
                print("Shuffling detected") #TODO: reset stuff
            #...
        # TODO: Update GUI
        return

    @staticmethod
    def dealer_image_handler(self, image):
        cards : list[Card] = process_cards(image)
        #TODO: organizer call to update dealer hand
        #TODO: Update GUI
        return

    @staticmethod
    def player_image_handler(self, image):
        cards : list[Card] = process_cards(image)
        organize_players_cards(cards, self.table_state)
        #TODO: Update GUI
        return