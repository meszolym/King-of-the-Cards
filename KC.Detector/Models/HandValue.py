from dataclasses import dataclass

@dataclass
class HandValue:
    value: int
    is_soft: bool
    is_pair: bool
    is_blackjack: bool
    is_bust: bool