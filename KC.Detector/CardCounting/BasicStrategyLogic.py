import json
from ..Models.BasicStrategy import BasicStrategy
from ..Models.Enums import Move
from ..Models.HandValue import HandValue
from HandValueLogic import hand_value_from_string

def read_strategy(filepath: str) -> BasicStrategy:
    strategy = BasicStrategy()
    strategy.strategy_table = {}
    with open(filepath, "r") as f:
        data = json.load(f)
        strategy.upper_bound = data.get("UpperBoundary", 0)
        strategy.lower_bound = data.get("LowerBoundary", 0)
        for entry in data.get("Entries", []):
            hand_value = hand_value_from_string(entry["HandValue"])
            dealer_card = int(entry["DealerCard"])
            move = Move[entry["Move"]]
            strategy.strategy_table[(hand_value, dealer_card)] = move

    return strategy

def get_hand_actions(strategy: BasicStrategy, hand_value: HandValue, dealer_upcard: int) -> Move:
    move = strategy.strategy_table.get((hand_value, dealer_upcard), Move.Unknown)
    if move != Move.Unknown:
        return move

    if hand_value.value > strategy.upper_bound:
        return Move.Stand
    if hand_value.value < strategy.lower_bound:
        return Move.Hit

    return Move.Unknown