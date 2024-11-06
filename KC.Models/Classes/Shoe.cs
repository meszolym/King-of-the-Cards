using KC.Models.Structs;

namespace KC.Models.Classes;

public class Shoe(Card[] cards)
{
    public Card[] Cards { get; private set; } = cards;
    public int NextCardIdx; //index of the card that will be dealt next
    public int ShuffleCardIdx = -1; //invalid index
}