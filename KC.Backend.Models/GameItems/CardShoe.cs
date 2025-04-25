using KC.Shared.Models.GameItems;

namespace KC.Backend.Models.GameItems;

public class CardShoe(List<Card> cards)
{
    public List<Card> Cards { get; } = cards;
    public int NextCardIdx { get; set; } = 0; //index of the card that will be dealt next
    public int ShuffleCardIdx { get; set; }= -1; //invalid index, to be set when shuffling
    public int OriginalShuffleCardIdx { get; init; }
    public int OriginalShuffleCardRange { get; init; }
}