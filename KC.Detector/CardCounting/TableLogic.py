from ..Models.Table import Table
from ..Models.Enums import Rank
from ..Models.Hand import Hand

class TableLogic:
    @staticmethod
    def hilo_running_count(table: Table) -> int:
        count = 0
        for card in table.played_cards:
            if card.rank.value in [2, 3, 4, 5, 6]: #low cards
                count += 1
            elif card.rank in [Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace]: #high cards
                count -= 1
        return count

    @staticmethod
    def hilo_true_count(table: Table) -> int:
        return TableLogic.hilo_running_count(table) / (len(table.played_cards) / 52) if table.played_cards else 0

    @staticmethod
    def reset(table: Table) -> None:
        table.hands = []
        table.dealer_hand = Hand()
        table.played_cards = []