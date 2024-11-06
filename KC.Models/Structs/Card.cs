using KC.Models.Enums;

namespace KC.Models.Structs;

public readonly struct Card(CardSuit suit, CardFace face)
{
    public CardSuit Suit { get; } = suit;
    public CardFace Face { get; } = face;
}