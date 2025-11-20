import pytest

from Models.Card import Card
from Models.BoundingBox import BoundingBox
from Models.Enums import Rank
from Models.Hand import Hand

from CardCounting.HandValueLogic import hand_value_from_hand, hand_value_from_string
from Models.HandValue import HandValue
from Tests.CardCounting.Utils import make_card


@pytest.mark.parametrize(
    "input_str, expected",
    [
        ("P10", HandValue(value=20, is_soft=False, is_pair=True, is_blackjack=False, is_bust=False)),
        ("S18", HandValue(value=18, is_soft=True, is_pair=False, is_blackjack=False, is_bust=False)),
        ("21", HandValue(value=21, is_soft=False, is_pair=False, is_blackjack=False, is_bust=False)),
        ("10", HandValue(value=10, is_soft=False, is_pair=False, is_blackjack=False, is_bust=False)),
        ("P1", HandValue(value=2, is_soft=False, is_pair=True, is_blackjack=False, is_bust=False)),
    ],
)
def test_hand_value_from_string(input_str: str, expected: HandValue):
    got = hand_value_from_string(input_str)
    assert got == expected


@pytest.mark.parametrize(
    "cards, expected",
    [
        # None hand
        (None, HandValue(value=0, is_soft=False, is_pair=False, is_blackjack=False, is_bust=False)),
        # empty hand
        ([], HandValue(value=0, is_soft=False, is_pair=False, is_blackjack=False, is_bust=False)),
        # single Ace
        ([make_card(Rank.Ace)], HandValue(value=11, is_soft=True, is_pair=False, is_blackjack=False, is_bust=False)),
        # pair of Aces -> should be adjusted to 12 and remain soft
        ([make_card(Rank.Ace), make_card(Rank.Ace)], HandValue(value=12, is_soft=True, is_pair=True, is_blackjack=False, is_bust=False)),
        # Ace + King -> blackjack (2-card 21)
        ([make_card(Rank.Ace), make_card(Rank.King)], HandValue(value=21, is_soft=True, is_pair=False, is_blackjack=True, is_bust=False)),
        # King + Queen + Two -> bust 10+10+2 = 22
        ([make_card(Rank.King), make_card(Rank.Queen), make_card(Rank.Two)], HandValue(value=22, is_soft=False, is_pair=False, is_blackjack=False, is_bust=True)),
        # Ace + Nine -> 20 soft
        ([make_card(Rank.Ace), make_card(Rank.Nine)], HandValue(value=20, is_soft=True, is_pair=False, is_blackjack=False, is_bust=False)),
        # Ace + Ace + Nine -> 21 (one ace becomes 1)
        ([make_card(Rank.Ace), make_card(Rank.Ace), make_card(Rank.Nine)], HandValue(value=21, is_soft=True, is_pair=False, is_blackjack=False, is_bust=False)),
        # Pair of Fives
        ([make_card(Rank.Five), make_card(Rank.Five)], HandValue(value=10, is_soft=False, is_pair=True, is_blackjack=False, is_bust=False)),
        # Unknown rank card
        ([make_card(Rank.Unknown)], HandValue(value=0, is_soft=False, is_pair=False, is_blackjack=False, is_bust=False)),
    ],
)
def test_hand_value_from_hand(cards, expected: HandValue):
    if cards is None:
        hand = None
    else:
        hand = Hand(cards=cards, bottom_center_x=0, bottom_center_y=0)

    got = hand_value_from_hand(hand)
    assert got == expected
