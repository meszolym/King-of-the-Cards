#Do the processing of cards (rank, suit, etc.) here
import os
import re
from typing import Optional

import numpy as np
from Models.Card import Card
from Models.BoundingBox import BoundingBox
import cv2 as cv

from Models.CardSizesContainer import CardSizesContainer
from Models.Enums import CardType, Rank, Suit


class CardProcessor:
    CONST_CANNY_THRESHOLD1 = 250
    CONST_CANNY_THRESHOLD2 = 250
    CONST_KERNEL_SIZE = (3, 3)
    CONST_ITERATIONS = 2
    CONST_CONTOUR_AREA_PCT_MIN = 0.9
    CONST_CONTOUR_AREA_PCT_MAX = 1.1

    card_sizes: Optional[CardSizesContainer]

    def __init__(self):
        self.card_sizes = None
        return


    @staticmethod
    def _show_boxes(img, boxes, message):
        for box in boxes:
            cv.rectangle(img, (int(box.x), int(box.y)), (int(box.x + box.w), int(box.y + box.h)), (0, 255, 0), 2)
            if message:
                cv.putText(img, message, (int(box.x), int(box.y) - 10), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)
        cv.imshow("Detected Cards", img)
        cv.waitKey(0)
        cv.destroyAllWindows()

    def process_cards(self, img : np.ndarray, card_type: CardType) -> list[Card]:
        if self.card_sizes is None:
            raise Exception("Card sizes not set")

        if card_type not in [CardType.Dealer, CardType.Player]:
            raise ValueError(f"Invalid card type: {card_type}. Valid types are CardType.Dealer and CardType.Player. Please pass a valid card type to process_cards().")

        cards : list[Card] = []
        approx_size = self.card_sizes.dealer_card if card_type == CardType.Dealer else self.card_sizes.player_card
        boxes = self.find_card_boxes(img, round(approx_size))

        # self._show_boxes(img.copy(), boxes,"") # for debugging

        for box in boxes:
            cards.append(self.process_card(img, box))
        return cards

    def find_card_boxes(self, img: np.ndarray, approx_size: int) -> list[BoundingBox]:
        gray = cv.cvtColor(img.copy(), cv.COLOR_BGR2GRAY)
        canny = cv.Canny(gray, self.CONST_CANNY_THRESHOLD1,self.CONST_CANNY_THRESHOLD2)
        dilated = cv.dilate(canny,cv.getStructuringElement(cv.MORPH_ELLIPSE, self.CONST_KERNEL_SIZE), iterations = self.CONST_ITERATIONS)
        contours, hierarchy = cv.findContours(dilated, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)

        # debug_img = img.copy()
        # cv.drawContours(debug_img, contours, -1, (255, 0, 0), 2)
        # cv.imshow("All Contours", debug_img)
        # cv.waitKey(0)
        # cv.destroyAllWindows()

        card_boxes : list[BoundingBox] = []

        for contour in contours:
            if approx_size*self.CONST_CONTOUR_AREA_PCT_MIN < cv.contourArea(contour) < approx_size*self.CONST_CONTOUR_AREA_PCT_MAX:
                x, y, w, h = cv.boundingRect(contour)
                card_boxes.append(BoundingBox(x,y,w,h))

        return card_boxes

    @staticmethod
    def process_card(img: np.ndarray, box: BoundingBox) -> Card:
        # Extract the top-left region of the card
        card_top_left = img[
            int(box.y):int(box.y + box.h / 3),
            int(box.x):int(box.x + box.w / 3)
        ].copy()

        directory = "Assets/CardsCut/"
        best_score = float('-inf')
        best_match_name = None

        card_top_left_gray = cv.cvtColor(card_top_left, cv.COLOR_BGR2GRAY)

        for filename in os.listdir(directory):
            if not filename.lower().endswith(('.png', '.jpg', '.jpeg')):
                continue
            template_path = os.path.join(directory, filename)
            template_img = cv.imread(template_path)
            if template_img is None:
                continue
            template_img_gray = cv.cvtColor(template_img, cv.COLOR_BGR2GRAY)
            res = cv.matchTemplate(card_top_left_gray, template_img_gray, cv.TM_CCOEFF_NORMED)
            _, max_val, _, _ = cv.minMaxLoc(res)
            if max_val > best_score:
                best_score = max_val
                best_match_name = filename

        suit, rank = CardProcessor.parse_filename(best_match_name or "")

        return Card(rank, suit, box)

    @staticmethod
    def parse_filename(filename: str) -> tuple[Suit,Rank]:
        suit_map = {
            "Hearts": Suit.Hearts,
            "Diamonds": Suit.Diamonds,
            "Clubs": Suit.Clubs,
            "Spades": Suit.Spades,
        }
        rank_map = {
            "A": Rank.Ace,
            "2": Rank.Two,
            "3": Rank.Three,
            "4": Rank.Four,
            "5": Rank.Five,
            "6": Rank.Six,
            "7": Rank.Seven,
            "8": Rank.Eight,
            "9": Rank.Nine,
            "T": Rank.Ten,
            "10": Rank.Ten,
            "J": Rank.Jack,
            "Q": Rank.Queen,
            "K": Rank.King,
        }

        match = re.match(r'card([A-Za-z]+)([A2-9TJQK]|10)\.png', filename)
        if match:
            suit_str, rank_str = match.groups()
            suit = suit_map.get(suit_str, Suit.Unknown)
            rank = rank_map.get(rank_str, Rank.Unknown)
        else:
            suit, rank = Suit.Unknown, Rank.Unknown
        return suit, rank