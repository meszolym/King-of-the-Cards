namespace KC.Backend.Models.GameItems;

public record struct Card
{
    public enum CardSuit
    {
        None,
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }
    public enum CardFace
    {
        None = 0,
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }
    
    public CardSuit Suit { get; set; }
    public CardFace Face { get; set; }
    
    public int GetValue() => this switch
    {
        { Face: Card.CardFace.King } => 10,
        { Face: Card.CardFace.Jack } => 10,
        { Face: Card.CardFace.Queen } => 10,
        _ => (int)this.Face
    };
    
}