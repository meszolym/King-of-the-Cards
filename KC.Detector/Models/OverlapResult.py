from Card import Card
class OverlapResult:
    percentage: float
    card_from_hand: Card
    smaller_card: Card

    def __init__(self, percentage: float = 0.0, card_from_hand: Card = Card(), smaller_card: Card = Card()):
        self.percentage = percentage
        self.card_from_hand = card_from_hand
        self.smaller_card = smaller_card