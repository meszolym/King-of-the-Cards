from dataclasses import dataclass
from Models.BoundingBox import BoundingBox
from Models.Enums import Rank, Suit
@dataclass
class Card:
    rank: Rank
    suit: Suit
    box: BoundingBox