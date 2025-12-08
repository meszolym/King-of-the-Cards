from Models.BoundingBox import BoundingBox
from Models.Card import Card
from Models.Enums import Suit, Rank
from Models.Hand import Hand
from Models.Table import Table


def make_card(rank=Rank.Unknown, suit=Suit.Unknown, x=0, y=0, w=0, h=0, conf=None):
    return Card(rank=rank, suit=suit, box=BoundingBox(x, y, w, h), recognition_confidence=conf)

def make_table(play_ranks: list[Rank], dealer_ranks: list[Rank] | None = None, hands_ranks: list[list[Rank]] | None = None) -> Table:
    played_cards = [make_card(r) for r in play_ranks]
    dealer_hand = None
    if dealer_ranks is not None:
        dealer_hand = Hand(cards=[make_card(r) for r in dealer_ranks], bottom_center_x=0, bottom_center_y=0)
    hands = []
    if hands_ranks is not None:
        for hr in hands_ranks:
            hands.append(Hand(cards=[make_card(r) for r in hr], bottom_center_x=0, bottom_center_y=0))
    return Table(dealer_hand=dealer_hand, played_cards=played_cards, hands=hands)