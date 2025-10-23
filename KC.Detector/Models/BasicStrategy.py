from dataclasses import dataclass
from Models.Enums import Move
from Models.HandValue import HandValue

@dataclass
class BasicStrategyEntry:
    hand_value: HandValue
    dealer_upcard: int
    move: Move

@dataclass
class BasicStrategy:
    strategy_list: list[BasicStrategyEntry]
    lower_bound: int
    upper_bound: int
