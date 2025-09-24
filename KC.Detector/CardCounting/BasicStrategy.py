#Implement basic strategy here, read from file, then find the necessary action for the player vs dealer upcard
from Models.Enums import Move
from Models.HandValue import HandValue
from Models.Card import Card
import json
class BasicStrategy:
    strategy_table: dict[tuple[HandValue, int], Move]
    lower_bound: int
    upper_bound: int

    def __init__(self, filepath: str):
        self.strategy_table = {}
        with open(filepath, "r") as f:
            data = json.load(f)
            self.upper_bound = data.get("UpperBoundary", 0)
            self.lower_bound = data.get("LowerBoundary", 0)
            for entry in data.get("Entries", []):
                hand_value = HandValue.from_string(entry["HandValue"])
                dealer_card = int(entry["DealerCard"])
                move = Move[entry["Move"]]
                self.strategy_table[(hand_value, dealer_card)] = move

    def get_actions(self, hand_value: HandValue, dealer_upcard: int) -> Move:
        if hand_value.value > self.upper_bound:
            return Move.Stand
        if hand_value.value < self.lower_bound:
            return Move.Hit

        return self.strategy_table.get((hand_value, dealer_upcard), Move.Unknown)
