using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Classes;
using KC.Models.Structs;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;

namespace KC.Logic.SessionLogic.TableLogic.ShoeLogic;

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