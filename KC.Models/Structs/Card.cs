using KC.Models.Enums;

namespace KC.Models.Structs;

public readonly record struct Card(CardSuit Suit, CardFace Face);