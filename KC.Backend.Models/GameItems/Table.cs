using KC.Backend.Models.Dtos;

namespace KC.Backend.Models.GameItems;

public class Table
{
    public Dealer Dealer { get; } = new ();
    public List<BettingBox> BettingBoxes { get; private init; }
    public CardShoe Shoe { get; set; }

    public Table(int numberOfBoxes, CardShoe shoe)
    {
        BettingBoxes = Enumerable.Range(0, numberOfBoxes).Select(i => new BettingBox() {IdxOnTable = i}).ToList();
        Shoe = shoe;
    }
}