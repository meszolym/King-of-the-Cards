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
    Unknown = 0
    Hit = 1
    Stand = 2
    DoubleHit = 3
    DoubleStand = 4
    Split = 5

class Message(IntEnum):
    Unknown = 0
    Shuffling = 1
    WaitingForBets = 2