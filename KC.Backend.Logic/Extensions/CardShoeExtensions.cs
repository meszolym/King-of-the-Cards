using System;
using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Extensions;

public static class CardShoeExtensions
{
    public static void ResetShuffleCardPlacement(this CardShoe shoe, Random? random = null)
    {
        random ??= Random.Shared;
        shoe.ShuffleCardIdx = random.Next(shoe.OriginalShuffleCardIdx - shoe.OriginalShuffleCardRange,shoe.OriginalShuffleCardIdx - shoe.OriginalShuffleCardRange);
        
        if (shoe.ShuffleCardIdx < 0)
            shoe.ShuffleCardIdx += shoe.Cards.Count;
    }
}