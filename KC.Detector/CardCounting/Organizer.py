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
        table.played_cards.extend(detected_cards)
        return

    for card in detected_cards:
        found_match = False

        for existing_card in table.dealer_hand.cards:
            updated = _check_and_update_same_card(existing_card, card)
            if updated:
                found_match = True
                break

        if not found_match:
            table.dealer_hand.cards.append(card)
            table.played_cards.append(card)


    return


def organize_players_cards(detected_cards: list[Card], table: Table, x_offset, y_offset) -> None:
    if not table.hands:
        table.hands = []

    split_count = 0
    if len(table.hands) > 0:
        split_count = len(detected_cards) - len(table.hands)

    for card in detected_cards:
        placed = False
        for hand in table.hands:


            if not hand.cards:
                hand.cards = []
                continue

            last_card = hand.cards[-1]

            # Case 1: Same card detected again: box matches, we have the same rank and suit, or better confidence
            # Case 2: New card overlapping with last card in hand, we add it to that hand
            # Case 3: Replaced card: box matches, but different rank/suit (no matter the confidence) -> Split happened, new top card appeared
            #   and the old top card is moved to a different place
            # Case 4: New card that does not overlap with any existing hand, move to next hand (or create new hand if none match)

            if boxes_match(last_card.box, card.box):
                if split_count == 0 or (last_card.rank == card.rank and last_card.suit == card.suit):
                    # Case 1: Same card detected again
                    _check_and_update_same_card(last_card, card)
                else:
                    # Case 3: Replaced card due to split
                    table.played_cards.remove(next (x for x in table.played_cards
                                                    if x.suit == hand.cards[-1].suit
                                                    and x.rank == hand.cards[-1].rank
                                                    and boxes_match(x.box, hand.cards[-1].box))) #TODO: Check this
                    hand.cards[-1] = card
                    table.played_cards.append(card)
                    split_count -= 1

                placed = True
                break

            if _overlap(card, hand):
                # Case 2: New card overlapping with the existing hand
                hand.cards.append(card)
                table.played_cards.append(card)
                placed = True
                break

            # Continue to next hand if no match found (Case 4)

        if not placed:
            # Create new hand for this card (Case 4)
            new_hand = Hand([card],
                            x_offset + int(card.box.x + card.box.w // 2),
                            y_offset + int(card.box.y + card.box.h))
            table.played_cards.append(card)
            table.hands.append(new_hand)

    table.hands.sort(key=lambda h: h.bottom_center_x, reverse=True)
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
