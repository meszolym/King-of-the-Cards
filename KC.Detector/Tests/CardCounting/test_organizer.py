import pytest

from CardCounting.Organizer import organize_dealer_cards, organize_players_cards
from Models.BoundingBox import BoundingBox
from Models.Enums import Rank, Suit
from Models.Hand import Hand
from Models.Table import Table
from Tests.CardCounting.Utils import make_card


@pytest.mark.parametrize("x_offset,y_offset", [(0, 0), (10, 5)])
def test_organize_dealer_initial_sets_dealer_hand(x_offset, y_offset):
    """When dealer_hand is None, the first detected card becomes the dealer hand and is added to played_cards."""
    detected = [make_card(Rank.Ace, Suit.Spades, 100, 10, 20, 30, conf=0.8)]
    table = Table(None, [], [])

    organize_dealer_cards(detected, table, x_offset, y_offset)

    assert table.dealer_hand is not None
    assert table.dealer_hand.cards == detected
    # bottom center x is x_offset + int(leftmost_card.box.x + leftmost_card.box.w // 2)
    expected_bottom_center_x = x_offset + int(detected[0].box.x + detected[0].box.w // 2)
    expected_bottom_center_y = y_offset + int(detected[0].box.y + detected[0].box.h)
    assert table.dealer_hand.bottom_center_x == expected_bottom_center_x
    assert table.dealer_hand.bottom_center_y == expected_bottom_center_y
    assert table.played_cards == detected


@pytest.mark.parametrize(
    "initial_box,new_box,initial_conf,new_conf,initial_rank,new_rank,expect_box_updated,expect_rank_updated",
    [
        # New box is smaller -> expect box updated
        (BoundingBox(50, 5, 10, 10), BoundingBox(50, 5, 8, 8), 0.5, 0.4, Rank.Ace, Rank.Ace, True, False),
        # New recognition confidence higher and different rank -> expect rank updated
        (BoundingBox(60, 5, 10, 10), BoundingBox(60, 5, 10, 10), 0.3, 0.9, Rank.Two, Rank.Three, False, True),
    ],
)
def test_organize_dealer_updates_same_card(
    initial_box, new_box, initial_conf, new_conf, initial_rank, new_rank, expect_box_updated, expect_rank_updated
):
    """If the same card is detected again, Organizer should update box/rank when appropriate and not duplicate played_cards."""
    old_card = make_card(initial_rank, Suit.Hearts, initial_box.x, initial_box.y, initial_box.w, initial_box.h, conf=initial_conf)
    table = Table(Hand([old_card], 0, 0), [old_card], [])

    # new card detected with same box location (boxes_match should be true for identical boxes)
    new_card = make_card(new_rank, Suit.Hearts, new_box.x, new_box.y, new_box.w, new_box.h, conf=new_conf)

    organize_dealer_cards([new_card], table, 0, 0)

    # played_cards should not gain a duplicate when updating existing dealer card
    assert len(table.played_cards) == 1

    if expect_box_updated:
        assert table.dealer_hand.cards[0].box.w == new_box.w
        assert table.dealer_hand.cards[0].box.h == new_box.h
    else:
        assert table.dealer_hand.cards[0].box.w == initial_box.w

    if expect_rank_updated:
        assert table.dealer_hand.cards[0].rank == new_rank
        assert table.dealer_hand.cards[0].recognition_confidence == new_conf
    else:
        assert table.dealer_hand.cards[0].rank == initial_rank


@pytest.mark.parametrize(
    "existing_box,new_box,should_append",
    [
        # Overlapping but not matching -> should append to same hand
        (BoundingBox(10, 10, 20, 30), BoundingBox(15, 12, 18, 25), True),
        # Non-overlapping -> should create a new hand
        (BoundingBox(10, 10, 20, 30), BoundingBox(100, 100, 20, 30), False),
    ],
)
def test_organize_players_append_or_new_hand(existing_box, new_box, should_append):
    """If a detected player card overlaps an existing hand it should be appended; otherwise a new hand is created."""
    # existing hand with one card
    existing_card = make_card(Rank.Five, Suit.Clubs, existing_box.x, existing_box.y, existing_box.w, existing_box.h, conf=0.6)
    hand = Hand([existing_card], existing_box.x + existing_box.w // 2, existing_box.y + existing_box.h)
    table = Table(None, [existing_card], [hand])

    new_card = make_card(Rank.Six, Suit.Clubs, new_box.x, new_box.y, new_box.w, new_box.h, conf=0.7)

    organize_players_cards([new_card], table, 0, 0)

    if should_append:
        assert len(table.hands) == 1
        assert table.hands[0].cards[-1] == new_card
        assert new_card in table.played_cards
    else:
        # a new hand should be created
        assert len(table.hands) == 2
        assert any(h.cards[0] == new_card for h in table.hands)
        assert new_card in table.played_cards


def test_organize_players_split_replaces_old_card():
    """Handle split case: when boxes_match but rank changed and split_count > 0, old card should be removed from played_cards and replaced."""
    # initial hand with one card
    old_card = make_card(Rank.Seven, Suit.Diamonds, 20, 20, 10, 10, conf=0.5)
    hand = Hand([old_card], 25, 30)
    # table.played_cards includes the original old_card (so card_to_remove can be found)
    table = Table(None, [old_card], [hand])

    # detected_cards length 2 => split_count = 2 - len(table.hands) = 1 -> allows split handling
    # new top card has same box (so boxes_match) but different rank
    new_top = make_card(Rank.Ace, Suit.Diamonds, 20, 20, 10, 10, conf=0.9)
    other = make_card(Rank.Ten, Suit.Clubs, 200, 5, 10, 10, conf=0.6)

    organize_players_cards([new_top, other], table, 0, 0)

    # old_card should have been removed from played_cards and replaced in the hand
    assert all(not (c.rank == old_card.rank and c.suit == old_card.suit and c.box.w == old_card.box.w and c.box.h == old_card.box.h) for c in table.played_cards)
    # hands may be re-ordered by bottom_center_x sorting; find the hand that contains the new_top card
    assert any(h.cards and h.cards[-1] == new_top for h in table.hands)
    assert new_top in table.played_cards
