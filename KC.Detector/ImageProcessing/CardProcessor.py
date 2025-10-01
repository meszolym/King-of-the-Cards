#Do the processing of cards (rank, suit, etc.) here
from typing import Optional

import numpy as np
from Models.Card import Card
from Models.BoundingBox import BoundingBox
import cv2 as cv

from Models.CardSizesContainer import CardSizesContainer
from Models.Enums import CardType


class CardProcessor:

    CONST_KERNEL_SIZE = (3,3)
    CONST_ITERATIONS = 2
    CONST_CANNY_THRESHOLD1 = 200
    CONST_CANNY_THRESHOLD2 = 200
    CONST_CONTOUR_AREA_PCT_MIN = 0.9
    CONST_CONTOUR_AREA_PCT_MAX = 1.1

    card_sizes: Optional[CardSizesContainer]

    def __init__(self):
        self.card_sizes = None
        return
    def process_cards(self, img : np.ndarray, card_type: CardType) -> list[Card]:
        if self.card_sizes is None:
            raise Exception("Card sizes not set")

        if card_type not in [CardType.Dealer, CardType.Player]:
            raise ValueError(f"Invalid card type: {card_type}. Valid types are CardType.Dealer and CardType.Player. Please pass a valid card type to process_cards().")

        cards : list[Card] = []
        approx_size = self.card_sizes.dealer_card if card_type == CardType.Dealer else self.card_sizes.player_card
        gray = cv.cvtColor(img.copy(), cv.COLOR_BGR2GRAY)
        canny = cv.Canny(gray, self.CONST_CANNY_THRESHOLD1,self.CONST_CANNY_THRESHOLD2)
        dilated = cv.dilate(canny,cv.getStructuringElement(cv.MORPH_ELLIPSE, self.CONST_KERNEL_SIZE), iterations = self.CONST_ITERATIONS)
        contours, hierarchy = cv.findContours(dilated, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)

        card_boxes : list[BoundingBox] = []

        for contour in contours:
            if approx_size*self.CONST_CONTOUR_AREA_PCT_MIN < cv.contourArea(contour) < approx_size*self.CONST_CONTOUR_AREA_PCT_MAX:
                epsilon = 0.1*cv.arcLength(contour,True)
                approx = cv.approxPolyDP(contour,epsilon,True)
                x, y, w, h = cv.boundingRect(contour)
                card_boxes.append(BoundingBox(x,y,w,h))

        #TODO: Implement card processing here
        return cards