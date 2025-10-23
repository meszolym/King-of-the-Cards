#Do organization of cards into hands here
from typing import Optional

from Models.Table import Table
from Models.Hand import Hand
from Models.Card import Card
from CardCounting.BoundingBoxLogic import *

def organize_dealer_cards(detected_cards: list[Card], table: Table, x_offset, y_offset) -> None:
    if table.dealer_hand is None:
        leftmost_card = min(detected_cards, key=lambda c: c.box.x)
        table.dealer_hand = Hand(detected_cards,
                                 x_offset + int(leftmost_card.box.x + leftmost_card.box.w // 2),
                                 y_offset + int(leftmost_card.box.y + leftmost_card.box.h))
        return

    for card in detected_cards:
        for existing_card in table.dealer_hand.cards:
            updated = _check_and_update_same_card(existing_card, card)
            if updated:
                break

            table.dealer_hand.cards.append(card)

    return


def organize_players_cards(detected_cards: list[Card], table: Table, x_offset, y_offset) -> None:
    #Organize cards based on the coordinates.

    #If no hands exist, create a new hand for each detected card
    if not table.hands:
        for c in detected_cards:
            hand = Hand([c],
                        x_offset + int(c.box.x + c.box.w // 2),
                        y_offset + int(c.box.y + c.box.h))
            table.hands.append(hand)
        return

    #Go through each card, and check against each hand to find the one that has a card (latest) with a contour overlapping this card
    split_origin: Optional[Hand] = None
    for c in detected_cards:
        assigned = False

        # First, try to find the matching card in existing hands
        for h in table.hands:
            for existing_card in h.cards:
                updated = _check_and_update_same_card(existing_card, c)
                if updated:
                    assigned = True
                    split_origin = h
                    break
            if assigned:
                break

        if not assigned:
            for h in table.hands:
                if _overlap(c, h) > 0:
                    h.cards.append(c)
                    assigned = True
                    break

        if not assigned:
            # Create new hand (split origin must exist to remove card from it)
            if split_origin.cards and c in split_origin.cards:
                split_origin.cards.remove(c)
            hand = Hand([c],
                        x_offset + int(c.box.x + c.box.w // 2),
                        y_offset + int(c.box.y + c.box.h))
            table.hands.append(hand)
            assigned = True
    return


def _check_and_update_same_card(old_card: Card, new_card: Card) -> bool:
    if boxes_match(old_card.box, new_card.box):
        # Update bounding box if new box is smaller (more accurate)
        if box_area(new_card.box) < box_area(old_card.box):
            old_card.box = new_card.box

        # Update rank and suit if different and new confidence is higher
        if (old_card.rank != new_card.rank or old_card.suit != new_card.suit) and new_card.recognition_confidence > old_card.recognition_confidence:
            old_card.rank, old_card.suit, old_card.recognition_confidence = new_card.rank, new_card.suit, new_card.recognition_confidence

        return True
    return False


def _overlap(card: Card, hand: Hand) -> float:
    max_overlap = 0.0

    for hand_card in hand.cards:
        overlap_ratio = boxes_overlap(card.box, hand_card.box)
        if overlap_ratio > max_overlap:
            max_overlap = overlap_ratio

    return max_overlap
