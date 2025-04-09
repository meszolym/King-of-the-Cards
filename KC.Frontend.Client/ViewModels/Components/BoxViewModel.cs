using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
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

    private readonly ExternalCommunicatorService _externalCommunicator;
    private readonly int _boxIdx;

    //TODO: This should reflect if the game is in progress or not (you cannot unclaim a box that is in use)
    private IObservable<bool> CanClaimDisclaimBox => Observable.Return(true);
    
    private static Subject<Unit> _boxClaimStatusChanged = new Subject<Unit>();
    public static IObservable<Unit> BoxClaimStatusChanged => _boxClaimStatusChanged.AsObservable();

    [ReactiveCommand(CanExecute = nameof(CanClaimDisclaimBox))]
    private async Task ClaimBox()
    {
        try
        {
            await _externalCommunicator.ClaimBox(_sessionId, _boxIdx, ClientMacAddressHandler.PrimaryMacAddress);
            var localPlayer = Locator.Current.GetRequiredService<PlayerViewModel>();
            PlayerName = localPlayer.PlayerName;
            OwnerId = localPlayer.Id;
            IsClaimed = true;
            _boxClaimStatusChanged.OnNext(Unit.Default);
            
        }
        catch (Exception e)
        {
            //TODO: Show dialog
            Debug.WriteLine(e.Message + "at ClaimBox in BoxViewModel");
        }
    }   

    [ReactiveCommand(CanExecute = nameof(CanClaimDisclaimBox))]
    private async Task DisclaimBox()
    {
        try
        {
            await _externalCommunicator.DisclaimBox(_sessionId, _boxIdx, ClientMacAddressHandler.PrimaryMacAddress);
            PlayerName = "Unclaimed";
            OwnerId = Guid.Empty;
            IsClaimed = false;
            _boxClaimStatusChanged.OnNext(Unit.Default);
        }
        catch (Exception e)
        {
            //TODO: Show dialog
            Debug.WriteLine(e.Message + "at DisclaimBox in BoxViewModel");
        }
    }
    
    //TODO: Claim box button command (done) + canexecute (skeleton done) + server comm
    public BoxViewModel(Guid sessionId, int boxIdx)
    {
        _sessionId = sessionId;
        _boxIdx = boxIdx;
        LeftHand = new HandViewModel();
        RightHand = new HandViewModel();
        IsSplit = false;
        PlayerName = "Unclaimed";
        _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
    }

    private readonly Guid _sessionId;


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