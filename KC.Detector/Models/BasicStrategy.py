from dataclasses import dataclass
from Models.Enums import Move
from Models.HandValue import HandValue
@dataclass
class BasicStrategy:
    strategy_table: dict[tuple[HandValue, int], Move]
    lower_bound: int
    upper_bound: int
