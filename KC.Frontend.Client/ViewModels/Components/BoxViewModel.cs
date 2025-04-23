using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
using KC.Shared.Models.Dtos;
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
    
    private IObservable<bool> IsPlayerOwned => this.WhenAnyValue(vm => vm.OwnerId)
        .Select(ownerId => ownerId == LocalPlayer.Id);

    //Betting modifier is visible when:
    // 1. The box is claimed by the player and the game is not in progress
    public IObservable<bool> IsBettingModifierVisible =>
        IsPlayerOwned.CombineLatest(CanClaimDisclaimBox,
            (isOwner, canClaim) => isOwner && canClaim);

    //Betting text is visible when:
    // 1. The box is not claimed by the player (e.g. unclaimed or claimed by another player)
    // 2. The game is in progress (this is effectively conveyed by CanClaimDisclaimBox)
    // 3. The box's hand is not split (if split, the betting text is shown on the hand level not the box level)
    public IObservable<bool> IsBettingTextVisible => IsPlayerOwned.CombineLatest(CanClaimDisclaimBox,
        this.WhenAnyValue(vm => vm.IsSplit), (p,c,s) => !p || s || !c);
    
    private static readonly Subject<Unit> BoxClaimStatusChangedSub = new Subject<Unit>();
    public static IObservable<Unit> BoxClaimStatusChanged => BoxClaimStatusChangedSub.AsObservable();

    [ReactiveCommand(CanExecute = nameof(CanClaimDisclaimBox))]
    private async Task ClaimBox()
    {
        try
        {
            await _externalCommunicator.ClaimBox(_sessionId, _boxIdx, ClientMacAddressHandler.PrimaryMacAddress);
            PlayerName = LocalPlayer.PlayerName;
            OwnerId = LocalPlayer.Id;
            IsClaimed = true;
            BoxClaimStatusChangedSub.OnNext(Unit.Default);
            
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
            BoxClaimStatusChangedSub.OnNext(Unit.Default);
        }
        catch (Exception e)
        {
            //TODO: Show dialog
            Debug.WriteLine(e.Message + "at DisclaimBox in BoxViewModel");
        }
    }
    public PlayerViewModel LocalPlayer => Locator.Current.GetRequiredService<PlayerViewModel>();
    public BoxViewModel(Guid sessionId, BettingBoxReadDto sourceDto)
    {
        _sessionId = sessionId;
        _boxIdx = sourceDto.BoxIdx;
        var hands = sourceDto.Hands.ToImmutableArray();
        RightHand = new HandViewModel(hands[0]);
        LeftHand = hands.Length > 1 ?  new HandViewModel(hands[1]) : new HandViewModel();
        
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
    public void UpdateBetAmount(decimal? argsOldValue, decimal? argsNewValue)
    {
        
    }
}