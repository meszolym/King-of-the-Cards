from dataclasses import dataclass
from typing import Optional

from CardCounting.BasicStrategyLogic import get_hand_actions
from CardCounting.CardLogic import card_value
from CardCounting.HandValueLogic import hand_value_from_hand
from CardCounting.TableLogic import hilo_running_count, hilo_true_count
from Models import Enums
from Models.BasicStrategy import BasicStrategy
from Models.Hand import Hand
from Models.Table import Table

@dataclass
class HandRecord:
    hand: Hand
    recommended_move: Optional[Enums.Move]

@dataclass
class OverlayModel:
    player_hand_info: list[HandRecord]
    dealer_hand_info: HandRecord
    table_running_count: int
    table_true_count: int

def overlay_data_from_table(table: Table, basic_strategy: BasicStrategy) -> OverlayModel:
    player_hand_info = []
    for hand in table.hands:
        if hand is not None:
            x = hand.bottom_center_x
            y = hand.bottom_center_y
            score = hand_value_from_hand(hand).__str__()
            move = get_hand_actions(basic_strategy, hand_value_from_hand(hand), card_value(table.dealer_hand.cards[0]))
            player_hand_info.append(HandRecord(x, y, score, move))

    dealer_x, dealer_y, dealer_score = (0, 0, "")
    if table.dealer_hand is not None:
        dealer_x = table.dealer_hand.bottom_center_x
        dealer_y = table.dealer_hand.bottom_center_y
        dealer_score = hand_value_from_hand(table.dealer_hand).__str__()

    running_count = hilo_running_count(table)
    true_count = hilo_true_count(table)

    return OverlayModel(
        player_hand_info=player_hand_info,
        dealer_hand_info=HandRecord(dealer_x, dealer_y, dealer_score, None),
        table_running_count=running_count,
        table_true_count=true_count
    )
