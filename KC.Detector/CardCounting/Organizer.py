#Do organization of cards into hands here
from Models.Table import Table
from Models.Hand import Hand
from Models.Card import Card
from CardCounting.BoundingBoxLogic import *

def organize_dealer_cards(detected_cards: list[Card], table: Table) -> None:
    if table.dealer_hand is None:
        table.dealer_hand = Hand()
        table.dealer_hand.cards = detected_cards
        return

    for c in detected_cards:
        assigned = False
        for existing_card in table.dealer_hand.cards:
            if (existing_card.rank == c.rank and existing_card.suit == c.suit and
                boxes_match(existing_card.box, c.box)):
                # Update bounding box if new box is smaller (more accurate)
                if box_area(c.box) < box_area(existing_card.box):
                    existing_card.box = c.box
                assigned = True
                break
        if not assigned:
            table.dealer_hand.cards.append(c)

    return

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
    split_origin: Hand = Hand()
    for c in detected_cards:
        assigned = False

        # First, try to find the matching card in existing hands
        for h in table.hands:
            for existing_card in h.cards:
                if (existing_card.rank == c.rank and existing_card.suit == c.suit and
                    boxes_match(existing_card.box, c.box)):
                    # Update bounding box if new box is smaller (more accurate)
                    if box_area(c.box) < box_area(existing_card.box):
                        existing_card.box = c.box
                    assigned = True
                    split_origin = h
                    break
            if assigned:
                break

        if not assigned:
            for h in table.hands:
                if overlap(c, h) > 0:
                    h.cards.append(c)
                    assigned = True
                    break

        if not assigned:
            # Create new hand (split origin must exist to remove card from it)
            if split_origin.cards and c in split_origin.cards:
                split_origin.cards.remove(c)
            hand = Hand()
            hand.cards.append(c)
            table.hands.append(hand)
            assigned = True
    return

def overlap(card: Card, hand: Hand) -> float:
    max_overlap = 0.0

    for hand_card in hand.cards:
        overlap_ratio = boxes_overlap(card.box, hand_card.box)
        if overlap_ratio > max_overlap:
            max_overlap = overlap_ratio

    return max_overlap
