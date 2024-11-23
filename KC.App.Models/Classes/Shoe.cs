using KC.App.Models.Structs;

namespace KC.App.Models.Classes;

public class Shoe(List<Card> cards)
{
    public List<Card> Cards { get; set; } = cards;
    public int NextCardIdx; //index of the card that will be dealt next
    public int ShuffleCardIdx = -1; //invalid index
}