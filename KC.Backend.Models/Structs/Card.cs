using KC.App.Backend.Models.Enums;

namespace KC.App.Backend.Models.Structs;

public readonly record struct Card(CardSuit Suit, CardFace Face);