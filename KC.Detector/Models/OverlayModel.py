from dataclasses import dataclass
from typing import Optional

from CardCounting.BasicStrategyLogic import get_hand_actions
from CardCounting.CardLogic import card_value
from CardCounting.HandValueLogic import hand_value_from_hand
from CardCounting.TableLogic import hilo_running_count, hilo_true_count
from Models import Enums
from Models.BasicStrategy import BasicStrategy
from Models.Enums import Move
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
    table_true_count: float
    played_cards: int = 0

def overlay_data_from_table(table: Table, basic_strategy: BasicStrategy) -> OverlayModel:
    player_hand_info = []
    for hand in table.hands:
        if hand is not None:
            score = hand_value_from_hand(hand).__str__()
            if table.dealer_hand is not None and table.dealer_hand.cards and len(table.dealer_hand.cards) > 0:
                dealer_card_value = card_value(table.dealer_hand.cards[0])
            else:
                dealer_card_value = None
            move = get_hand_actions(basic_strategy, hand_value_from_hand(hand), dealer_card_value)
            player_hand_info.append(HandRecord(hand, move))

    dealer_x, dealer_y, dealer_score = (0, 0, "")
    if table.dealer_hand is not None:
        dealer_score = hand_value_from_hand(table.dealer_hand).__str__()

    running_count = hilo_running_count(table)
    true_count = hilo_true_count(table)

    return OverlayModel(
        player_hand_info=player_hand_info,
        dealer_hand_info=HandRecord(table.dealer_hand, Move.Unknown),
        table_running_count=running_count,
        table_true_count=true_count,
        played_cards=len(table.played_cards)
    )
