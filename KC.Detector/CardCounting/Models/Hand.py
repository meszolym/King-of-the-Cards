from Card import Card
from HandValue import HandValue

class Hand:
    cards: list[Card]
    def __init__(self):
        self.cards = []

    def add_card(self, card: Card) -> None:
        self.cards.append(card)

    def remove_card(self, card: Card) -> None:
        self.cards.remove(card)

    def clear(self) -> None:
        self.cards = []

    def get_value(self) -> HandValue:
        return HandValue.from_hand(self)

    def __str__(self):
        return ', '.join(str(card) for card in self.cards) + f" (Value: {self.get_value()})"