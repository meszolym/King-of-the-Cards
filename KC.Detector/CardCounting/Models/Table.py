from Hand import Hand
from Card import Card
from Enums import Rank
class Table:
    dealer_hand: Hand
    played_cards: list[Card]
    hands: list[Hand]
    def __init__(self):
        self.hands = [] # List of player hands
        self.dealer_hand = Hand()  # Dealer's hand
        self.played_cards = []  # List to hold played cards

    def hilo_running_count(self) -> int:
        count = 0
        for card in self.played_cards:
            if card.rank.value in [2, 3, 4, 5, 6]: #low cards
                count += 1
            elif card.rank in [Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace]: #high cards
                count -= 1
        return count

    def hilo_true_count(self) -> int:
        return self.hilo_running_count() / (len(self.played_cards) / 52) if self.played_cards else 0