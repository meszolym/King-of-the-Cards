using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels.Components
{
    public partial class SessionBoxControlsViewModel : ReactiveObject
    {
        private IObservable<bool> _isMyTurn;
        public SessionBoxControlsViewModel()
        {
            _isMyTurn = Observable.Return(true);
            _canSplitOnHand = Observable.Return(true);
            _canDoubleDownOnHand = Observable.Return(false);
        }
        
        [ReactiveCommand(CanExecute = nameof(_isMyTurn))]
        void HitOnHand()
        {
            
        }
        
        [ReactiveCommand(CanExecute = nameof(_isMyTurn))]
        void StandOnHand()
        {
            
        }

        private IObservable<bool> _canDoubleDownOnHand;
        [ReactiveCommand(CanExecute = nameof(_canDoubleDownOnHand))]
        void DoubleDownOnHand()
        {
            
        }

        private IObservable<bool> _canSplitOnHand;
        [ReactiveCommand(CanExecute = nameof(_canSplitOnHand))]
        void SplitOnHand()
        {
            
        }
    }
}
