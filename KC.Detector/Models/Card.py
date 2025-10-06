from dataclasses import dataclass
from Models.BoundingBox import BoundingBox
from Models.Enums import Rank, Suit
from typing import Optional
@dataclass
class Card:
    rank: Rank
    suit: Suit
    box: BoundingBox
    recognition_confidence: Optional[float]