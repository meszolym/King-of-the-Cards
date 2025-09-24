from Enums import Rank
from Hand import Hand
class HandValue:
    value: int
    is_soft: bool
    is_pair: bool
    is_blackjack: bool
    is_bust: bool

    def __init__(self):
        return

    @staticmethod
    def from_string (value: str) -> 'HandValue':
        handval = HandValue()
        handval.is_soft = value[0] == 'S'
        handval.is_pair = value[0] == 'P'
        handval.is_blackjack = False
        handval.is_bust = False
        handval.value = int(value[1:])

        if handval.is_pair: # if it's a pair, the incoming value represents the rank of the pair
            handval.value = handval.value * 2

        return handval
    @staticmethod
    def from_hand (hand: Hand) -> 'HandValue':
        handval = HandValue()
        handval.value = 0
        aces = 0
        for card in hand.cards:
            handval.value += card.value()
            if card.rank == Rank.Ace:
                aces += 1

        # Adjust for aces if value exceeds 21
        while handval.value > 21 and aces:
            handval.value -= 10
            aces -= 1

        handval.is_soft = aces > 0 and handval.value <= 21
        handval.is_pair = len(hand.cards) == 2 and hand.cards[0].rank == hand.cards[1].rank
        handval.is_blackjack = len(hand.cards) == 2 and handval.value == 21
        handval.is_bust = handval.value > 21
        return handval