from ..Models.Card import Card
from ..Models.Enums import Rank

class CardLogic:
    @staticmethod
    def value(card: Card) -> int:
        if card.rank in [Rank.Jack, Rank.Queen, Rank.King]:
            return 10
        elif card.rank == Rank.Ace:
            return 11  # or 1, depending on the game rules
        elif card.rank == Rank.Unknown:
            return 0
        else:
            return card.rank.value