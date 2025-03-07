using KC.Backend.Models;

namespace KC.Backend.Logic.GameItemsLogic;

public static class CardUtilities
{
        /// <summary>
        /// Gets the value of a card. Aces are counted as 1.
        /// </summary>
        /// <param name="card"></param>
        /// <returns>The value of the card in blackjack. Aces are counted as 1.</returns>
        public static int GetValue(this Card card) => card switch
        {
            { Face: Card.CardFace.King } => 10,
            { Face: Card.CardFace.Jack } => 10,
            { Face: Card.CardFace.Queen } => 10,
            _ => (int)card.Face
        };
}
