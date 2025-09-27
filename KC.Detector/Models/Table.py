from Models.Hand import Hand
from Models.Card import Card
from Models.Enums import Rank
from dataclasses import dataclass
@dataclass
class Table:
    dealer_hand: Hand
    played_cards: list[Card]
    hands: list[Hand]