using KC.App.Models.Structs;
using LanguageExt;

namespace KC.App.Models.Classes;

public class Shoe(Seq<Card> cards)
{
    public Seq<Card> Cards { get; set; } = cards;
    public int NextCardIdx; //index of the card that will be dealt next
    public int ShuffleCardIdx = -1; //invalid index
}