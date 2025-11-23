from dataclasses import dataclass

from Models.Card import Card
from Models.Hand import Hand


@dataclass
class Table:
    dealer_hand: Hand
    played_cards: list[Card]
    hands: list[Hand]