from dataclasses import dataclass

from Models.BoundingBox import BoundingBox


@dataclass
class CardSizesContainer:
    dealer_card_box: BoundingBox
    player_card_box: BoundingBox