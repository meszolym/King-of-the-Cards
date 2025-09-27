from Models.HandValue import HandValue
from Models.Hand import Hand
from Models.Enums import Rank
from CardCounting.CardLogic import card_value

def hand_value_from_string(value: str) -> HandValue:
    hand_value = HandValue()
    hand_value.is_soft = value[0] == 'S'
    hand_value.is_pair = value[0] == 'P'
    hand_value.is_blackjack = False
    hand_value.is_bust = False
    hand_value.value = int(value[1:])

    if hand_value.is_pair:  # if it's a pair, the incoming value represents the rank of the pair
        hand_value.value = hand_value.value * 2

    return hand_value



def hand_value_from_hand(hand: Hand) -> HandValue:
    hand_value = HandValue()
    hand_value.value = 0
    aces = 0
    for card in hand.cards:
        hand_value.value += card_value(card)
        if card.rank == Rank.Ace:
            aces += 1

    # Adjust for aces if value exceeds 21
    while hand_value.value > 21 and aces:
        hand_value.value -= 10
        aces -= 1

    hand_value.is_soft = aces > 0 and hand_value.value <= 21
    hand_value.is_pair = len(hand.cards) == 2 and hand.cards[0].rank == hand.cards[1].rank
    hand_value.is_blackjack = len(hand.cards) == 2 and hand_value.value == 21
    hand_value.is_bust = hand_value.value > 21
    return hand_value