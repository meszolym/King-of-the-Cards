#Do organization of cards into hands here
from Models.Table import Table
from Models.Hand import Hand
from Models.Card import Card
class Organizer:
    @staticmethod
    def organize_players_cards(detected_cards: list[Card], table: Table) -> None:
        #Organize cards based on the coordinates.

        #If no hands exist, create a new hand for each detected card
        if not table.hands:
            for c in detected_cards:
                hand = Hand()
                hand.add_card(c)
                table.hands.append(hand)
                return

        #Go through each card, and check against each hand to find the one that has a card (latest) with a contour overlapping this card
        for c in detected_cards:
            assigned: bool = False
            if not assigned:
                for h in table.hands:
                    if Organizer.overlap(c, h):
                        h.add_card(c)
                        assigned = True
                        break

            if not assigned: #If no overlap found, create a new hand for this card, as a split must have occurred. TODO: Find where the split came from and remove that card.
                hand = Hand()
                hand.add_card(c)
                table.hands.append(hand)
                assigned = True
        return

    @staticmethod
    def overlap(card: Card, hand: Hand) -> bool:
        return False