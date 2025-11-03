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


def hilo_true_count(table: Table) -> float:
    running_count = hilo_running_count(table)
    decks_played = len(table.played_cards) / 52.0
    decks_remaining = 8 - decks_played #TODO: assuming an 8-deck shoe

    return round(running_count/decks_remaining,2) if decks_remaining > 0 else 0.0


def reset(table: Table) -> None:
    table.hands.clear()
    if table.dealer_hand is not None:
        table.dealer_hand.cards.clear()
    table.played_cards.clear()