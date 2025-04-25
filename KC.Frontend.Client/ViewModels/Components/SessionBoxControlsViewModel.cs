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
            isMyTurnObs.CombineLatest(ExternalCommunicatorService.SignalREvents.TurnChanged)
                .Subscribe( async void (tup) => await UpdateSubjects(tup.First, tup.Second));
            //TODO: Check async void and exception handling
        }

        private readonly Guid _sessionId;
        private readonly ExternalCommunicatorService _externalCommunicatorService = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
        private async Task UpdateSubjects(bool isMyTurn, TurnInfo turnInfo)
        {
            var moves = (await _externalCommunicatorService.GetPossibleMovesOnHand(_sessionId,turnInfo.BoxIdx, turnInfo.HandIdx)).ToArray();
            
            _canHitOnHandSubject.OnNext(isMyTurn && moves.Contains(Move.Hit));
            _canStandOnHandSubject.OnNext(isMyTurn && moves.Contains(Move.Stand));
            _canDoubleDownOnHandSubject.OnNext(isMyTurn && moves.Contains(Move.Double));
            _canSplitOnHandSubject.OnNext(isMyTurn && moves.Contains(Move.Split));
            
        }
        
        private readonly Subject<bool> _canHitOnHandSubject = new();
        private IObservable<bool> CanHitOnHand => _canHitOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanHitOnHand))]
        void HitOnHand()
        {
            
        }
        
        private readonly Subject<bool> _canStandOnHandSubject = new();
        private IObservable<bool> CanStandOnHand => _canStandOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanStandOnHand))]
        void StandOnHand()
        {
            
        }

        private readonly Subject<bool> _canDoubleDownOnHandSubject = new();
        private IObservable<bool> CanDoubleDownOnHand => _canDoubleDownOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanDoubleDownOnHand))]
        void DoubleDownOnHand()
        {
            
        }

        private readonly Subject<bool> _canSplitOnHandSubject = new();
        private IObservable<bool> CanSplitOnHand => _canSplitOnHandSubject.AsObservable();
        [ReactiveCommand(CanExecute = nameof(CanSplitOnHand))]
        void SplitOnHand()
        {
            
        }
    }
}
