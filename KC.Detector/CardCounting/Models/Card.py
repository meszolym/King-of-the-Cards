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

    def value(self) -> int:
        if self.rank in [Rank.Jack, Rank.Queen, Rank.King]:
            return 10
        elif self.rank == Rank.Ace:
            return 11  # or 1, depending on the game rules
        elif self.rank == Rank.Unknown:
            return 0
        else:
            return self.rank.value