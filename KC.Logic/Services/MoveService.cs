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
        //public static PossibleActions GetPossibleActions(Hand hand)
        //{
        //    var actions = new PossibleActions();

        //    var handValue = _valueLogic.GetValue(hand);

        //    //no action on bust hands
        //    if (handValue.Value >= 21) return actions;

        //    //can hit on any hand that is not bust
        //    actions.CanHit = true;

        //    if (hand.Cards.Count == 2)
        //    {
        //        //no action on split aces (you get a card when you split aces, no more)
        //        if (hand.Cards[0].Face == CardFace.Ace && hand.FromSplit) return actions;

        //        //can double any 2 cards (even after splitting)
        //        actions.CanDouble = true;

        //        if (handValue.IsPair && !hand.FromSplit)
        //        {
        //            //can split any pair, defined by the face of the cards (no resplits!)
        //            actions.CanSplit = true;
        //        }
        //    }

        //    return actions;

        //}

    }
}
