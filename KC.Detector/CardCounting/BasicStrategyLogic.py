import json
from Models.BasicStrategy import BasicStrategy
from Models.Enums import Move
from Models.HandValue import HandValue
from CardCounting.HandValueLogic import hand_value_from_string


def read_strategy(filepath: str) -> BasicStrategy:
    strategy = []
    upper_bound = 0
    lower_bound = 0
    with open(filepath, "r") as f:
        data = json.load(f)
        upper_bound = data.get("UpperBoundary", 0)
        lower_bound = data.get("LowerBoundary", 0)
        for entry in data.get("Entries", []):
            hand_value = hand_value_from_string(entry["HandValue"])
            dealer_card = int(entry["DealerCard"])
            move = Move[entry["Move"]]
            strategy.append((hand_value, dealer_card, move))

    return BasicStrategy(strategy_list=strategy, upper_bound=upper_bound, lower_bound=lower_bound)


def get_hand_actions(strategy: BasicStrategy, hand_value: HandValue, dealer_upcard: int) -> Move:
    move_entry = next((x for x in strategy.strategy_list if x[0] == hand_value and x[1] == dealer_upcard), None)

    move = move_entry[2] if move_entry is not None else Move.Unknown

    if move != Move.Unknown:
        return move

    if hand_value.value > strategy.upper_bound:
        return Move.Stand
    if hand_value.value < strategy.lower_bound:
        return Move.Hit

    return Move.Unknown