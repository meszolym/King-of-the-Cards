namespace KC.Backend.Models;

public class Table
{
    public Hand DealerHand { get; set; } = new();
    public List<BettingBox> BettingBoxes { get; private init; }
    public CardShoe Shoe { get; set; }

    public Table(int numberOfBoxes, List<Card> cardsForShoe)
    {
        BettingBoxes = Enumerable.Range(0, numberOfBoxes).Select(_ => new BettingBox()).ToList();
        Shoe = new(cardsForShoe);
    }
}