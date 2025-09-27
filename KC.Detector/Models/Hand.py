from Models.Card import Card

class Hand:
    cards: list[Card]
    def __init__(self):
        self.cards = []

    def __str__(self):
        return ', '.join(str(card) for card in self.cards) + f" (Value: {self.get_value()})"