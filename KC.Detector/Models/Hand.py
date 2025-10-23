from dataclasses import dataclass
from Models.Card import Card

@dataclass
class Hand:
    cards: list[Card]
    bottom_center_x: int
    bottom_center_y: int