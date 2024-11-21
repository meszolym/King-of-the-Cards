using KC.App.Models.Enums;

namespace KC.App.Models.Structs;

public readonly record struct Card(CardSuit Suit, CardFace Face);