using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.SourceGenerators;

namespace KC.App.Frontend.ViewModels
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
