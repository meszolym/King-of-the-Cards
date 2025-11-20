import pytest

from CardCounting.TableLogic import hilo_running_count, hilo_true_count, reset, DECK_LENGTH, NUMBER_OF_DECKS

from Models.Enums import Rank
from Tests.CardCounting.Utils import make_table


@pytest.mark.parametrize(
    "ranks,expected",
    [
        ([], 0),
        ([Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six], 5),
        ([Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace], -5),
        ([Rank.Two, Rank.Seven, Rank.Ten, Rank.Five, Rank.Ace], 0),
        ([Rank.Seven, Rank.Eight, Rank.Nine], 0),
    ],
)
def test_hilo_running_count(ranks, expected):
    table = make_table(ranks)
    assert hilo_running_count(table) == expected


@pytest.mark.parametrize(
    "ranks",
    [
        [],
        [Rank.Two],
        [Rank.Ten, Rank.Jack],
        [Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six],
        [Rank.Seven, Rank.Eight, Rank.Nine],
    ],
)
def test_hilo_true_count_matches_manual_calculation(ranks):
    table = make_table(ranks)
    running = hilo_running_count(table)
    decks_played = len(table.played_cards) / DECK_LENGTH
    decks_remaining = NUMBER_OF_DECKS - decks_played
    expected = round(running / decks_remaining, 2) if decks_remaining > 0 else 0.0
    assert hilo_true_count(table) == expected


def test_hilo_true_count_returns_zero_when_no_decks_remaining():
    # Create a table with exactly NUMBER_OF_DECKS * DECK_LENGTH played cards -> decks_remaining == 0
    total_cards = NUMBER_OF_DECKS * DECK_LENGTH
    ranks = [Rank.Two] * total_cards  # content doesn't matter for this case
    table = make_table(ranks)
    assert hilo_true_count(table) == 0.0


def test_reset_clears_table_state():
    # Build a table with hands, dealer hand and played cards
    played = [Rank.Two, Rank.Three]
    dealer = [Rank.Ace]
    hands = [[Rank.Four, Rank.Five], [Rank.Six]]
    table = make_table(played, dealer_ranks=dealer, hands_ranks=hands)

    # Sanity checks before reset
    assert len(table.played_cards) == 2
    assert table.dealer_hand is not None and len(table.dealer_hand.cards) == 1
    assert len(table.hands) == 2 and all(len(h.cards) > 0 for h in table.hands)

    # Perform reset and verify
    reset(table)
    assert table.played_cards == []
    assert table.hands == []
    if table.dealer_hand is not None:
        assert table.dealer_hand.cards == []
