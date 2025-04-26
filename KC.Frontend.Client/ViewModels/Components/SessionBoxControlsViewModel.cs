using System;
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
        public SessionBoxControlsViewModel(Guid sessionId, IObservable<bool> isMyTurnObs)
        {
            _sessionId = sessionId;
            isMyTurnObs.CombineLatest(ExternalCommunicatorService.SignalREvents.TurnChanged, ExternalCommunicatorService.SignalREvents.HandsUpdated)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe( async void (tup) => await Update(tup.First, tup.Second));
            //TODO: Check async void and exception handling
        }

        private readonly Guid _sessionId;
        private readonly ExternalCommunicatorService _externalCommunicatorService = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
        private TurnInfo _currentTurnInfo;
        private async Task Update(bool isMyTurn, TurnInfo turnInfo)
        {
            _currentTurnInfo = turnInfo;
            
            if (!isMyTurn)
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
                moves = (await _externalCommunicatorService.GetPossibleMovesOnHand(_sessionId, turnInfo.BoxIdx,
                    turnInfo.HandIdx)).ToArray();
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
            async Task HitOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_sessionId, _currentTurnInfo.BoxIdx, Move.Hit, _currentTurnInfo.HandIdx);
        
        private readonly Subject<bool> _canStandOnHandSubject = new();
        private IObservable<bool> CanStandOnHand => _canStandOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanStandOnHand))]
            async Task StandOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_sessionId, _currentTurnInfo.BoxIdx, Move.Stand, _currentTurnInfo.HandIdx);

        private readonly Subject<bool> _canDoubleDownOnHandSubject = new();
        private IObservable<bool> CanDoubleDownOnHand => _canDoubleDownOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanDoubleDownOnHand))]
            async Task DoubleDownOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_sessionId, _currentTurnInfo.BoxIdx, Move.Double, _currentTurnInfo.HandIdx);

        private readonly Subject<bool> _canSplitOnHandSubject = new();
        private IObservable<bool> CanSplitOnHand => _canSplitOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanSplitOnHand))]
            async Task SplitOnHand() => await _externalCommunicatorService.MakeMoveOnHand(_sessionId, _currentTurnInfo.BoxIdx, Move.Split, _currentTurnInfo.HandIdx);
    }
}
