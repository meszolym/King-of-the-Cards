from enum import IntEnum
class Rank(IntEnum):
    Unknown = 0
    Ace = 1
    Two = 2
    Three = 3
    Four = 4
    Five = 5
    Six = 6
    Seven = 7
    Eight = 8
    Nine = 9
    Ten = 10
    Jack = 11
    Queen = 12
    King = 13

class Suit(IntEnum):
    Unknown = 0
    Hearts = 1
    Diamonds = 2
    Clubs = 3
    Spades = 4

class Move(IntEnum):
    Hit = 0
    Stand = 1
    Double = 2
    Split = 3