from Models.Hand import Hand
from Models.Card import Card
from Models.Enums import Rank
class Table:
    dealer_hand: Hand
    played_cards: list[Card]
    hands: list[Hand]
    def __init__(self):
        self.hands = [] # List of player hands
        self.dealer_hand = Hand()  # Dealer's hand
        self.played_cards = []  # List to hold played cards