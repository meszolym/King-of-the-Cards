namespace KC.Backend.Models;

public class CardShoe(List<Card> cards)
{
    public List<Card> Cards { get; } = cards;
    public int NextCardIdx = 0; //index of the card that will be dealt next
    public int ShuffleCardIdx = -1; //invalid index, to be set when shuffling
}