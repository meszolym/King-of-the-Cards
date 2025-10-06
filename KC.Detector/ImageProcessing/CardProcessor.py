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
    CONST_CANNY_CARD_EDGE_THRESHOLD1, CONST_CANNY_CARD_EDGE_THRESHOLD2 = 250, 250
    CONST_KERNEL_SIZE = (3, 3)
    CONST_ITERATIONS = 2
    CONST_CONTOUR_AREA_PCT_MIN = 0.9
    CONST_CONTOUR_AREA_PCT_MAX = 1.1

    card_sizes: Optional[CardSizesContainer]

    def __init__(self):
        self.card_sizes = None
        return


    @staticmethod
    def _show_cards(img, cards):
        for card in cards:
            cv.rectangle(img, (int(card.box.x), int(card.box.y)), (int(card.box.x + card.box.w), int(card.box.y + card.box.h)), (0, 255, 0), 2)
            cv.putText(img, f"{card.rank.name} of {card.suit.name} ({card.recognition_confidence})", (int(card.box.x), int(card.box.y) - 10), cv.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)
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

        if len(cards) != 0: #for debugging
            self._show_cards(img, cards)

        return cards

    def find_card_boxes(self, img: np.ndarray, approx_size: int) -> list[BoundingBox]:
        gray = cv.cvtColor(img.copy(), cv.COLOR_BGR2GRAY)
        canny = cv.Canny(gray, self.CONST_CANNY_CARD_EDGE_THRESHOLD1, self.CONST_CANNY_CARD_EDGE_THRESHOLD2)
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
        card_total = img[
            int(box.y):int(box.y + box.h),
            int(box.x):int(box.x + box.w)
        ].copy()
        card_total = cv.cvtColor(card_total, cv.COLOR_BGR2GRAY)
        card_total = cv.threshold(card_total, 120, 255, cv.THRESH_BINARY)[1]

        crop_height_total, crop_width_total = card_total.shape
        target_size_total = (crop_width_total, crop_height_total)

        card_top_left = card_total[:card_total.shape[0]//3, :card_total.shape[1]//3].copy()
        crop_height_top_left, crop_width_top_left = card_top_left.shape
        target_size_top_left = (crop_width_top_left, crop_height_top_left)

        directory = "Assets/Cards/"
        best_score = float('-inf')
        best_match_name = None

        for filename in os.listdir(directory):
            if not filename.lower().endswith(('.png', '.jpg', '.jpeg')):
                continue
            template_path = os.path.join(directory, filename)
            template_total = cv.imread(template_path)
            if template_total is None:
                continue

            template_total = cv.cvtColor(template_total, cv.COLOR_BGR2GRAY)
            template_total = cv.resize(template_total, target_size_total)
            template_total = cv.threshold(template_total, 120, 255, cv.THRESH_BINARY)[1]

            template_top_left = template_total[:template_total.shape[0]//3, :template_total.shape[1]//3]
            template_top_left = cv.resize(template_top_left, target_size_top_left)

            res_top_left = cv.matchTemplate(card_top_left, template_top_left, cv.TM_CCOEFF_NORMED)
            _, max_val_top_left, _, _ = cv.minMaxLoc(res_top_left)

            res_total = cv.matchTemplate(card_total, template_total, cv.TM_CCOEFF_NORMED)
            _, max_val_total, _, _ = cv.minMaxLoc(res_total)

            if (max_val_top_left+max_val_total)/2 > best_score:
                best_score = (max_val_top_left+max_val_total)/2
                best_match_name = filename

        suit, rank = CardProcessor.parse_filename(best_match_name or "")

        if best_score < 0.6:
            suit, rank = Suit.Unknown, Rank.Unknown

        return Card(rank, suit, box, best_score)

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