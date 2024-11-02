using KC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Logic.Services
{
    public static class MoveService
    {
        public static PossibleActions GetPossibleActions(Hand hand)
        {
            bool canHit = false;
            bool canDouble = false;
            bool canSplit = false;

            var handValue = hand.GetValue();

            //no action on bust hands
            if (handValue.Value >= 21) return new PossibleActions(canHit, canDouble, canSplit);

            //can hit on any hand that is not bust
            canHit = true;

            if (hand.Cards.Count == 2)
            {
                //no action on split aces (you get a card when you split aces, no more)
                if (hand.Cards[0].Face == CardFace.Ace && !hand.Splittable) return new PossibleActions(canHit, canDouble, canSplit);

                //can double any 2 cards (even after splitting)
                canDouble = true;

                if (handValue.IsPair && hand.Splittable)
                {
                    //can split any pair, defined by the face of the cards (no resplits!)
                    canSplit = true;
                }
            }

            return new PossibleActions(canHit, canDouble, canSplit);

        }

    }
}
