using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.GameManagement;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace KC.Frontend.Client.ViewModels.Components
{
    public partial class SessionBoxControlsViewModel : ReactiveObject
    {
        /// <summary>
        /// Partial class for the SessionBoxControlsViewModel which contains ReactiveUI ReactiveCommand initialization.
        /// </summary>
        public SessionBoxControlsViewModel(SessionViewModel parent)
        {
            _parent = parent;
            ExternalCommunicatorService.SignalREvents.HandsUpdated.ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ => Debug.WriteLine("Hands updated"));
            ExternalCommunicatorService.SignalREvents.TurnChanged.ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ => Debug.WriteLine("Turn changed"));
            
            Observable.CombineLatest(
                ExternalCommunicatorService.SignalREvents.HandsUpdated.ObserveOn(RxApp.MainThreadScheduler),
                ExternalCommunicatorService.SignalREvents.TurnChanged.ObserveOn(RxApp.MainThreadScheduler),
                (_, turnInfo) => turnInfo
            ).Subscribe(turnInfo => Update(turnInfo).ConfigureAwait(false));
        }
        
        private readonly ExternalCommunicatorService _externalCommunicatorService = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
        private readonly PlayerViewModel _playerViewModel = Locator.Current.GetRequiredService<PlayerViewModel>();
        private TurnInfo _currentTurnInfo;
        private readonly SessionViewModel _parent;
        private async Task Update(TurnInfo turnInfo)
        {
            _currentTurnInfo = turnInfo;
            
            if (!_parent.Boxes.Any(b => b.BoxTurnState != BoxViewModel.TurnState.None && b.OwnerId == _playerViewModel.Id))
            {
                _canHitOnHandSubject.OnNext(false);
                _canStandOnHandSubject.OnNext(false);
                _canDoubleDownOnHandSubject.OnNext(false);
                _canSplitOnHandSubject.OnNext(false);
                return;
            }

            Move[] moves;
            try
            {
                moves = (await _externalCommunicatorService.GetPossibleMovesOnHand(_parent.Id, turnInfo.BoxIdx, turnInfo.HandIdx)).ToArray();
            }
            catch (Exception e)
            {
                moves = [];
            }
            
            _canHitOnHandSubject.OnNext(moves.Contains(Move.Hit));
            _canStandOnHandSubject.OnNext(moves.Contains(Move.Stand));
            _canDoubleDownOnHandSubject.OnNext(moves.Contains(Move.Double));
            _canSplitOnHandSubject.OnNext(moves.Contains(Move.Split));
            
        }
        
        private readonly Subject<bool> _canHitOnHandSubject = new();
        private IObservable<bool> CanHitOnHand => _canHitOnHandSubject.AsObservable();

        [ReactiveCommand(CanExecute = nameof(CanHitOnHand))]
            async Task HitOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_parent.Id, _currentTurnInfo.BoxIdx, Move.Hit, _currentTurnInfo.HandIdx);
        
        private readonly Subject<bool> _canStandOnHandSubject = new();
        private IObservable<bool> CanStandOnHand => _canStandOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanStandOnHand))]
            async Task StandOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_parent.Id, _currentTurnInfo.BoxIdx, Move.Stand, _currentTurnInfo.HandIdx);

        private readonly Subject<bool> _canDoubleDownOnHandSubject = new();
        private IObservable<bool> CanDoubleDownOnHand => _canDoubleDownOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanDoubleDownOnHand))]
            async Task DoubleDownOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_parent.Id, _currentTurnInfo.BoxIdx, Move.Double, _currentTurnInfo.HandIdx);

        private readonly Subject<bool> _canSplitOnHandSubject = new();
        private IObservable<bool> CanSplitOnHand => _canSplitOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanSplitOnHand))]
            async Task SplitOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_parent.Id, _currentTurnInfo.BoxIdx, Move.Split, _currentTurnInfo.HandIdx);
    }
}
