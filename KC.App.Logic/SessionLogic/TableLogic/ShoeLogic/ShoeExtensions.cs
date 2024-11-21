using KC.App.Models.Classes;
using KC.App.Models.Structs;
using LanguageExt;

namespace KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;

public static class ShoeExtensions
{
    public static Unit Shuffle(this Shoe shoe, Random random)
    {
        // Fischer-Yates shuffle
        var cardsMutable = shoe.Cards.ToArray();
        for (int i = 0; i < shoe.Cards.Length; i++)
        {
            int j = random.Next(i, shoe.Cards.Length);
            (cardsMutable[i], cardsMutable[j]) = (cardsMutable[j], cardsMutable[i]);
        }
        shoe.NextCardIdx = 0;
        shoe.Cards = new Seq<Card>(cardsMutable.AsEnumerable());
        return Unit.Default;
    }

    public static Card TakeCard(this Shoe shoe) => shoe.Cards[shoe.NextCardIdx++];
}