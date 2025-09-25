from Enums import Rank, Suit
class Card:
    rank: Rank
    suit: Suit
    coordinates: tuple[int, int, int, int]  # x, y, width, height
    def __init__(self, rank: Rank, suit: Suit):
        self.rank = rank
        self.suit = suit
        self.coordinates = (-1, -1, -1, -1)

    def __repr__(self):
        return f"{self.rank} of {self.suit}"