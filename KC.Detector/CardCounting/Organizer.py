#Do organization of cards into hands here
from ..Models.Table import Table
from ..Models.Hand import Hand
from ..Models.Card import Card
from ..Models.OverlapResult import OverlapResult
from BoundingBoxLogic import *

def organize_players_cards(detected_cards: list[Card], table: Table) -> None:
    #Organize cards based on the coordinates.

    #If no hands exist, create a new hand for each detected card

    if not table.hands:
        for c in detected_cards:
            hand = Hand()
            hand.cards.append(c)
            table.hands.append(hand)
            return

    #Go through each card, and check against each hand to find the one that has a card (latest) with a contour overlapping this card
    split_origin : Hand = Hand()
    for c in detected_cards:
        assigned: bool = False
        if not assigned:
            for h in table.hands:
                if c in h.cards:
                    split_origin = h #If found hand where this card already exists, mark it as the split origin (as this card was already detected in a previous frame)
                    assigned = True
                    break

                if overlap(c, h):
                    h.cards.append(c)
                    assigned = True
                    break

        if not assigned: #If no overlap found, create a new hand for this card, as a split must have occurred.
            split_origin.cards.remove(c)
            hand = Hand()
            hand.cards.append(c)
            table.hands.append(hand)
            assigned = True
    return

def overlap(card: Card, hand: Hand) -> OverlapResult:
    max_overlap = 0.0
    max_hand_card = Card()  # Empty card when no overlap
    max_smaller_card = Card()

    for hand_card in hand.cards:
        overlap_ratio = boxes_overlap(card.box, hand_card.box)
        if overlap_ratio > max_overlap:
            max_overlap = overlap_ratio
            max_hand_card = hand_card
            max_smaller_card = card if box_area(card.box) <= box_area(hand_card.box) else hand_card

    return OverlapResult(max_overlap, max_hand_card, max_smaller_card)
