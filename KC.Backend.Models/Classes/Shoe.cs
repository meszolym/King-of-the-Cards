using KC.App.Backend.Models.Structs;

namespace KC.App.Backend.Models.Classes;

public class Shoe(List<Card> cards)
{
    public List<Card> Cards { get; } = cards;
    public int NextCardIdx; //index of the card that will be dealt next
    public int ShuffleCardIdx = -1; //invalid index
}