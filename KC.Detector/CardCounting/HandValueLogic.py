from Models.HandValue import HandValue
from Models.Hand import Hand
from Models.Enums import Rank
from CardCounting.CardLogic import card_value


def hand_value_from_string(value: str) -> HandValue:
    return HandValue(value = int(value[1:])*2 if value[0] == 'P' else int(value[1:]),
                     is_soft = value[0] == 'S',
                     is_pair = value[0] == 'P',
                     is_blackjack = False,
                     is_bust = False)


def hand_value_from_hand(hand: Hand) -> HandValue:
    value = 0
    aces = 0
    for card in hand.cards:
        value += card_value(card)
        if card.rank == Rank.Ace:
            aces += 1

    # Adjust for aces if value exceeds 21
    while value > 21 and aces:
        value -= 10
        aces -= 1

    return HandValue(value = value,
                     is_soft = aces > 0 and value <= 21,
                     is_pair = len(hand.cards) == 2 and hand.cards[0].rank == hand.cards[1].rank,
                     is_blackjack = len(hand.cards) == 2 and value == 21,
                     is_bust = value > 21)