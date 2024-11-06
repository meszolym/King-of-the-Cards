using KC.Models.Classes;
using KC.Models.Structs;

namespace KC.Logic.ShoeLogic;

public class ShoeService
{
    public static Shoe CreateShoe(int numberOfDecks)
    {
        return new Shoe(new List<Card>());
    }
}