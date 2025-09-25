from BoundingBox import BoundingBox

from Enums import Rank, Suit
class Card:
    rank: Rank
    suit: Suit
    box: BoundingBox

    def __init__(self):
        self.rank = Rank.Unknown
        self.suit = Suit.Unknown
        self.box = BoundingBox()


    def __repr__(self):
        return f"{self.rank} of {self.suit}"