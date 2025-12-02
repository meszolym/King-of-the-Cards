from Models.HandValue import HandValue
from Models.Hand import Hand
from Models.Enums import Rank
from CardCounting.CardLogic import card_value


def hand_value_from_string(value: str) -> HandValue:
    value_number = 0
    bj = False
    soft = False
    match value[0]:
        case 'P' if value == 'P11':
            value_number = 12
            soft = True
        case 'P':
            value_number = int(value[1:]) * 2
        case 'S':
            value_number = int(value[1:])
            soft = True
        case 'B' if value == 'BJ':
            value_number = 21
            bj = True
        case _:
            value_number = int(value)

    return HandValue(value = value_number,
                     is_soft = soft,
                     is_pair = value[0] == 'P',
                     is_blackjack = bj,
                     is_bust = value_number > 21)


def hand_value_from_hand(hand: Hand) -> HandValue:
    if (hand is None) or (not hand.cards):
        return HandValue(value = 0,
                         is_soft = False,
                         is_pair = False,
                         is_blackjack = False,
                         is_bust = False)

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