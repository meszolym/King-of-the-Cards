from dataclasses import dataclass
from Models.Card import Card

@dataclass
class Hand:
    cards: list[Card]