using KC.App.Models.Classes;
using KC.App.Models.Structs;

namespace KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;

public static class ShoeExtensions
{
    public static void Shuffle(this Shoe shoe, Random random)
    {
        // Fischer-Yates shuffle
        for (int i = 0; i < shoe.Cards.Count; i++)
        {
            int j = random.Next(i, shoe.Cards.Count);
            (shoe.Cards[i], shoe.Cards[j]) = (shoe.Cards[j], shoe.Cards[i]);
        }
        shoe.NextCardIdx = 0;
    }

    public static Card TakeCard(this Shoe shoe) => shoe.Cards[shoe.NextCardIdx++];
}