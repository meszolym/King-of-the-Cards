from Card import Card
from Enums import Rank
class Hand:
    def __init__(self):
        self.cards = []

    def add_card(self, card: Card) -> None:
        self.cards.append(card)

    def remove_card(self, card: Card) -> None:
        self.cards.remove(card)

    def clear(self) -> None:
        self.cards = []

    def get_value(self) -> int:
        value = 0
        aces = 0
        for card in self.cards:
            value += card.value()
            if card.rank == Rank.Ace:
                aces += 1

        # Adjust for aces if value exceeds 21
        while value > 21 and aces:
            value -= 10
            aces -= 1

        return value

    def is_blackjack(self) -> bool:
        return len(self.cards) == 2 and self.get_value() == 21

    def is_bust(self) -> bool:
        return self.get_value() > 21

    def __str__(self):
        return ', '.join(str(card) for card in self.cards) + f" (Value: {self.get_value()})"