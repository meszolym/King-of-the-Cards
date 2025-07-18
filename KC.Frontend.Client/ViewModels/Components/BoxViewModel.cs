﻿using System;
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
using KC.Shared.Models.GameManagement;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace KC.Frontend.Client.ViewModels.Components;

public partial class BoxViewModel : ReactiveObject
{

    public enum TurnState
    {
        None,
        Right,
        Left
    }

    [Reactive]
    private TurnState _boxTurnState;
    
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

    private readonly ExternalCommunicatorService _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
    private readonly int _boxIdx;

    
    [Reactive]
    private bool _bettingPhase = true;
    
    public IObservable<bool> IsLocalPlayerOwned => this.WhenAnyValue(vm => vm.OwnerId)
        .Select(ownerId => ownerId == LocalPlayer.Id);

    private IObservable<bool> BettingPhaseObs => this.WhenAnyValue(vm => vm.BettingPhase);

    //Betting modifier is visible when:
    // 1. The box is claimed by the player and the game is not in progress
    public IObservable<bool> IsBettingModifierVisible =>
        IsLocalPlayerOwned.CombineLatest(BettingPhaseObs,
            (isOwner, bettingPhase) => isOwner && bettingPhase);

    //Betting text is visible when:
    // 1. The box is not claimed by the player (e.g. unclaimed or claimed by another player)
    // 2. The game is in progress (this is effectively conveyed by CanClaimDisclaimBox)
    // 3. The box's hand is not split (if split, the betting text is shown on the hand level not the box level)
    public IObservable<bool> IsBettingTextVisible => IsLocalPlayerOwned.CombineLatest(BettingPhaseObs,
        this.WhenAnyValue(vm => vm.IsSplit), (playerOwned,bettingPhase,isSplit) => (!playerOwned || !bettingPhase) && !isSplit);
    
    private static readonly Subject<Unit> BoxClaimStatusChangedSub = new Subject<Unit>();
    public static IObservable<Unit> BoxClaimStatusChanged => BoxClaimStatusChangedSub.AsObservable();

    [ReactiveCommand(CanExecute = nameof(BettingPhaseObs))]
    private async Task ClaimBox()
    {
        try
        {
            await _externalCommunicator.ClaimBox(_sessionId, _boxIdx);
            PlayerName = LocalPlayer.PlayerName;
            OwnerId = LocalPlayer.Id;
            BoxClaimStatusChangedSub.OnNext(Unit.Default);
            
        }
        catch (Exception e)
        {
            //TODO: Show dialog
            Debug.WriteLine(e.Message + "at ClaimBox in BoxViewModel");
        }
    }   

    [ReactiveCommand(CanExecute = nameof(BettingPhaseObs))]
    private async Task DisclaimBox()
    {
        try
        {
            RightHand.BetAmount = 0; //This goes through UpdateBetAmount (and thus the server) as it is bound to the UI!
            await _externalCommunicator.DisclaimBox(_sessionId, _boxIdx);
            PlayerName = "Unclaimed";
            OwnerId = Guid.Empty;
            BoxClaimStatusChangedSub.OnNext(Unit.Default);
        }
        catch (Exception e)
        {
            //TODO: Show dialog
            Debug.WriteLine(e.Message + "at DisclaimBox in BoxViewModel");
        }
    }
    public PlayerViewModel LocalPlayer => Locator.Current.GetRequiredService<PlayerViewModel>();
    public BoxViewModel(Guid sessionId, BettingBoxReadDto sourceDto, TurnInfo turnInfo, bool bettingPhase)
    {
        _sessionId = sessionId;
        _boxIdx = sourceDto.BoxIdx;
        
        var hands = sourceDto.Hands.ToImmutableArray();
        RightHand = new HandViewModel(hands[0]);
        LeftHand = hands.Length > 1 ?  new HandViewModel(hands[1]) : new HandViewModel();
        
        BettingPhase = bettingPhase;
        IsSplit = hands.Length > 1;
        
        PlayerName = sourceDto.OwnerName == string.Empty ? "Unclaimed" : sourceDto.OwnerName;
        OwnerId = sourceDto.OwnerId;
        BoxTurnState = GetTurnState(turnInfo);
            
        ExternalCommunicatorService.SignalREvents.BoxOwnerChanged.ObserveOn(RxApp.MainThreadScheduler).Subscribe(dto =>
        {
            if (dto.BoxIdx != _boxIdx) return;
            
            if (dto.OwnerId == Guid.Empty)
            {
                PlayerName = "Unclaimed";
                OwnerId = Guid.Empty;
            }
            else
            {
                PlayerName = dto.OwnerName;
                OwnerId = dto.OwnerId;
            }
        });
        
        ExternalCommunicatorService.SignalREvents.BetUpdated.ObserveOn(RxApp.MainThreadScheduler).Subscribe(dto =>
        {
            if (dto.BoxIdx != _boxIdx) return;
            
            RightHand.BetAmount = (decimal)dto.Hands.First().Bet;
            
            if (dto.Hands.Count() > 1)
                LeftHand.BetAmount = (decimal)dto.Hands.Last().Bet;
        });
        
        ExternalCommunicatorService.SignalREvents.BettingTimerElapsed.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => BettingPhase = false);
        
        ExternalCommunicatorService.SignalREvents.HandsUpdated.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => UpdateHands(s.Table.BettingBoxes.First(b => b.BoxIdx == _boxIdx)));
        
        ExternalCommunicatorService.SignalREvents.TurnChanged.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(dto => BoxTurnState = GetTurnState(dto));
        
        ExternalCommunicatorService.SignalREvents.BettingReset.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(dto =>
            {
                BettingPhase = dto.CanPlaceBets;
                RightHand = new HandViewModel(dto.Table.BettingBoxes.First(b => b.BoxIdx == _boxIdx).Hands.FirstOrDefault());
            });
    }

    private readonly Guid _sessionId;

    private TurnState GetTurnState(TurnInfo turnInfo) => turnInfo switch
    {
        { PlayersTurn: false } => TurnState.None,
        _ when turnInfo.BoxIdx != _boxIdx => TurnState.None,
        { HandIdx: 0 } => TurnState.Right,
        { HandIdx: 1 } => TurnState.Left,
        _ => TurnState.None
    };
    
    public async Task<bool> UpdateBetAmount(decimal? oldVal, decimal? newVal)
    {
        if (newVal is null) return false;
        if (oldVal == newVal) return true;
        
        try
        {
            await _externalCommunicator.UpdateBet(_sessionId, _boxIdx, (double) newVal);
            return true;
        }
        catch (Exception e)
        {
            //TODO: Show dialog
            Debug.WriteLine(e);
            return false;
        }
    }

    private void UpdateHands(BettingBoxReadDto boxDto)
    {
        ImmutableArray<HandReadDto> hands = [..boxDto.Hands];
        if (hands.Length > 0)
        {
            RightHand = new HandViewModel(hands[0]);
            if (hands.Length > 1)
            {
                LeftHand = new HandViewModel(hands[1])
                {
                    IsPartOfSplit = true
                };
                IsSplit = true;
                RightHand.IsPartOfSplit = true;
            }
            else
                LeftHand = new HandViewModel();
        }
        else
        {
            RightHand = new HandViewModel();
            LeftHand = new HandViewModel();
        }
    }
}