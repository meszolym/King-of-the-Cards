using KC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Logic.Services
{
    public static class ValueService
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

        public static HandValue GetValue(this Hand hand)
        {
            bool containsAce = false;
            int value = 0;

            hand.Cards.AsIterable().Iter(card =>
            {
                if (card.Face == CardFace.Ace) containsAce = true;
                value += card.GetValue();
            });

            bool isSoft = containsAce && value <= 11;

            //ace was counted as one in the card evaluation, so we add 10 to the value if possible.
            if (isSoft) value += 10;

            bool isBlackJack = false;
            bool isPair = false;

            //blackjack can't be a pair nor soft.
            if (containsAce && value == 21)
            {
                isBlackJack = true;
                isSoft = false;
                return new HandValue(value, isBlackJack, isPair, isSoft);
            }

            //pair is only possible with two cards of the same face, and not if the hand was split already.
            isPair = (hand.Cards.Count == 2 && hand.Cards[0].Face == hand.Cards[1].Face && hand.Splittable);

            return new HandValue(value, isBlackJack, isPair, isSoft);
        }
    }
}
