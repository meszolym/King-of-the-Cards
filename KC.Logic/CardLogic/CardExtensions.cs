using KC.Models.Enums;
using KC.Models.Structs;

namespace KC.Logic.CardLogic;

public static class CardExtensions
{
        /// <summary>
        /// Gets the value of a card. Aces are counted as 1.
        /// </summary>
        /// <param name="card"></param>
        /// <returns>The value of the card in blackjack. Aces are counted as 1.</returns>
        public static int GetValue(this Card card) => card switch
        {
            { Face: CardFace.King } => 10,
            { Face: CardFace.Jack } => 10,
            { Face: CardFace.Queen } => 10,
            _ => (int)card.Face
        };
}
