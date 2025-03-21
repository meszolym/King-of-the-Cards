﻿using KC.Shared.Models.GameItems;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels;

public partial class BoxViewModel : ReactiveObject
{
    [Reactive]
    private HandViewModel _leftHand;
    [Reactive]
    private HandViewModel _rightHand;
    [Reactive]
    private bool _isPlayerControlled;
    [Reactive]
    private bool _isSplit;
        
    public BoxViewModel()
    {
        LeftHand = new HandViewModel { IsActive = true };
        RightHand = new HandViewModel { IsActive = false };
        IsPlayerControlled = false;
        IsSplit = false;
    }
        
    public void SplitHands()
    {
        if (LeftHand.Cards.Count >= 2 && !IsSplit)
        {
            // Take the second card from left hand and move it to right hand
            // var cardToMove = LeftHand.Cards[1];
            // LeftHand.Cards.RemoveAt(1);
            // RightHand.Cards.Add(cardToMove);
            //     
            // // Copy bet amount
            // RightHand.BetAmount = LeftHand.BetAmount;
            // Activate right hand
            RightHand.Cards.Clear();
            RightHand.IsActive = true;
            IsSplit = true;
            RightHand.AddCard(new(Card.WithSuitAndFace(Card.CardSuit.Clubs, Card.CardFace.Ace)));
            RightHand.AddCard(new(Card.WithSuitAndFace(Card.CardSuit.Hearts, Card.CardFace.Jack)));
            
        }
    }
}