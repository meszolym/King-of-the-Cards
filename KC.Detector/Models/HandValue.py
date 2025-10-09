from dataclasses import dataclass

@dataclass
class HandValue:
    value: int
    is_soft: bool
    is_pair: bool
    is_blackjack: bool
    is_bust: bool

    def __str__(self):
        if self.is_blackjack:
            return 'BJ'
        if self.is_pair:
            return f'P{self.value // 2}'
        elif self.is_soft:
            return f'S{self.value}'
        else:
            return f'{self.value}'