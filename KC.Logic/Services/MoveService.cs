using KC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace KC.Logic.Services
{
    public static class MoveService
    {
        public static PossibleActions GetPossibleActions(this Hand hand)
            => (hand, value: hand.GetValue(), firstcard: hand.Cards[0]) switch
            {
                //no action on bust hands
                { value.Value: >= 21 }
                    => new PossibleActions(CanHit: false, CanDouble: false, CanSplit: false),

                //can split pairs, can double on any two cards
                { value.IsPair: true, hand.Splittable: true }
                    => new PossibleActions(CanHit: true, CanDouble: true, CanSplit: true),

                //no action on split aces
                { hand.Cards.Count: 2, firstcard.Face: CardFace.Ace, hand.Splittable: false }
                    => new PossibleActions(CanHit: false, CanDouble: false, CanSplit: false),

                //can double on any two cards
                { hand.Cards.Count: 2 } 
                    => new PossibleActions(CanHit: true, CanDouble: true, CanSplit: false),

                //can hit on any hand that is not bust
                _ => new PossibleActions(CanHit: true, CanDouble: false, CanSplit: false)

            };
    }
}
