#Implement basic strategy here, read from file, then find the necessary action for the player vs dealer upcard
from Models.Enums import Move
from Models.HandValue import HandValue
class BasicStrategy:
    strategy_table: dict[tuple[HandValue, int], Move]
    lower_bound: int
    upper_bound: int
