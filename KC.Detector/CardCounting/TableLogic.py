from Models.Enums import Rank
from Models.Table import Table


def hilo_running_count(table: Table) -> int:
    count = 0
    for card in table.played_cards:
        if card.rank.value in [2, 3, 4, 5, 6]: #low cards
            count += 1
        elif card.rank in [Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace]: #high cards
            count -= 1
    return count


def hilo_true_count(table: Table) -> int:
    return hilo_running_count(table) // (len(table.played_cards) // 52) if table.played_cards else 0


def reset(table: Table) -> None:
    table.hands.clear()
    if table.dealer_hand is not None:
        table.dealer_hand.cards.clear()
    table.played_cards.clear()