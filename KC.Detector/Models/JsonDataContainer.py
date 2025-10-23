from dataclasses import dataclass

from Models.CardSizesContainer import CardSizesContainer
from Models.RoisContainer import RoisContainer


@dataclass
class JsonDataContainer:
    rois_container: RoisContainer
    sizes_container: CardSizesContainer