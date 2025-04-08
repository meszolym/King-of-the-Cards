using System;
using System.Reactive.Linq;
using KC.Frontend.Client.Extensions;
using KC.Shared.Models.GameItems;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace KC.Frontend.Client.ViewModels.Components;

public partial class BoxViewModel : ReactiveObject
{
    [Reactive]
    private HandViewModel _leftHand;
    
    [Reactive]
    private HandViewModel _rightHand;
    
    [Reactive]
    private bool _isSplit;

    [Reactive]
    private string _playerName;
    
    
    [Reactive]
    private Guid _ownerId = Guid.Empty;
    
    [Reactive]
    private bool _isClaimed;

    //TODO: This should reflect if the game is in progress or not (you cannot unclaim a box that is in use)
    private IObservable<bool> CanClaimUnclaimBox => Observable.Return(true);

    [ReactiveCommand(CanExecute = nameof(CanClaimUnclaimBox))]
    private void ClaimBox()
    {
        var localPlayer = Locator.Current.GetRequiredService<PlayerViewModel>();
        PlayerName = localPlayer.PlayerName;
        OwnerId = localPlayer.Id;
        IsClaimed = true;
    }   

    [ReactiveCommand(CanExecute = nameof(CanClaimUnclaimBox))]
    private void UnclaimBox()
    {
        PlayerName = "Unclaimed";
        OwnerId = Guid.Empty;
        IsClaimed = false;
    }
    
    //TODO: Claim box button command (done) + canexecute (skeleton done) + server comm
    public BoxViewModel(Guid sessionId, int boxIdx)
    {
        SessionId = sessionId;
        BoxIdx = boxIdx;
        LeftHand = new HandViewModel();
        RightHand = new HandViewModel();
        IsSplit = false;
        PlayerName = "Unclaimed";
    }

    public int BoxIdx { get; init; }

    public Guid SessionId { get; init; }


    //TODO: Take a look, this is more complex and needs to involve the server probably.
    //
    // public void SplitHands()
    // {
    //     if (LeftHand.Cards.Count >= 2 && !IsSplit)
    //     {
    //         // Take the second card from left hand and move it to right hand
    //         // var cardToMove = LeftHand.Cards[1];
    //         // LeftHand.Cards.RemoveAt(1);
    //         // RightHand.Cards.Add(cardToMove);
    //         //     
    //         // // Copy bet amount
    //         // RightHand.BetAmount = LeftHand.BetAmount;
    //         // Activate right hand
    //         RightHand.Cards.Clear();
    //         IsSplit = true;
    //
    //         RightHand.AddCard(Card.WithSuitAndFace(Card.CardSuit.Clubs, Card.CardFace.Ace));
    //         RightHand.AddCard(Card.WithSuitAndFace(Card.CardSuit.Hearts, Card.CardFace.Jack));
    //     }
    // }
}