using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Core.Interfaces;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameItems;

namespace KC.Backend.Logic.Core;

//This is done.
public class RuleBook : IRuleBook
{
    public List<Move> GetPossibleActionsOnHand(Hand hand)
    {
        if (hand.Cards.Count < 2) throw new InvalidOperationException("Cannot get actions for incomplete hand");
        if (hand.Finished) return []; //no action on finished hands
        if (GetValue(hand).NumberValue >= 21) return []; //no action on bust hands
        if (hand.Cards[0].Face == Card.CardFace.Ace && !CanBeSplit(hand)) return []; //no action on split aces (they automatically get only one card)

        var moves = new List<Move>
        {
            Move.Stand, //can always stand
            Move.Hit //can hit on any card
        };

        if (CanDouble(hand)) moves.Add(Move.Double); //can double on any two cards
        if (CanBeSplit(hand) && GetValue(hand).IsPair) moves.Add(Move.Split); //can split if the hand has not been split (splittable) and it is a pair
        return moves;
    }
    public HandValue GetValue(Hand hand)
    {
        bool containsAce = hand.Cards.Any(x => x.Face == Card.CardFace.Ace);
        int value = hand.Cards.Sum(x => x.GetValue());
        bool isSoft = containsAce && value <= 11;

        //ace was counted as one in the card evaluation, so we add 10 to the value if possible.
        if (isSoft) value += 10;

        bool isBlackJack = hand.Cards.Count() == 2 && containsAce && value == 21;
        bool isPair = hand.Cards.Count == 2 && hand.Cards[0].Face == hand.Cards[1].Face;

        return new HandValue(value, isBlackJack, isPair, isSoft);
    }
    
    public string GetHandValueString(Hand hand)
    {
        if (GetValue(hand).IsBlackJack) 
            return "BJ";
        
        if (hand.Finished) 
            return GetValue(hand).NumberValue.ToString();
        
        if (!hand.DealerOwned) 
            if (GetValue(hand).IsSoft && GetValue(hand).IsPair && CanBeSplit(hand)) 
                return "P11"; //Pair of Aces
        
        if (GetValue(hand).IsSoft && !hand.Finished) 
            return $"S{GetValue(hand).NumberValue}";
        
        if (hand.DealerOwned) 
            return GetValue(hand).NumberValue.ToString();
        
        if (GetValue(hand).IsPair && CanBeSplit(hand)) 
            return $"P{GetValue(hand).NumberValue / 2}";

        return GetValue(hand).NumberValue.ToString();
        
    }
    
    public bool CanBeSplit(Hand hand) => !hand.FromSplit;

    public bool CanDouble(Hand hand) => hand.Cards.Count == 2;
    
    public bool DealerShouldHit(Hand hand) => GetValue(hand).NumberValue < 17;

    public int BlackjackPayoutMultiplier { get; } = 3/2;
    
    public int StandardPayoutMultiplier { get; } = 1;
    
    public int BjVsBjPayoutMultiplier { get; } = 0;
    
    
}