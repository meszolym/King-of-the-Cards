using KC.Frontend.Client.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.ViewModels.Components;
using Splat;

namespace KC.Frontend.Client.ViewModels
{
    partial class SessionViewModel : ReactiveObject, IRoutableViewModel
    {
        
        [Reactive]
        private ObservableCollection<BoxViewModel> _boxes;
        
        [Reactive]
        private DealerViewModel _dealer;
        
        [ReactiveCommand]
        private void NavBack() => HostScreen.Router.NavigateBack.Execute();
        
        public SessionViewModel(IScreen hostScreen, Guid id)
        {
            this.HostScreen = hostScreen;
            Id = id;
            Boxes = new ObservableCollection<BoxViewModel>();
            Dealer = new DealerViewModel();
            InitializeBoxes();
            _player = Locator.Current.GetRequiredService<PlayerViewModel>();
        }

        public Guid Id { get; set; }

        public string? UrlPathSegment { get; } = "session";

        public IScreen HostScreen { get; }

        private PlayerViewModel _player;
        
        //TODO: This did not work :( Make it work
        //
        // private IObservable<bool> CanGoBack => BoxViewModel.BoxClaimStatusChanged.Select(_ =>
        // {
        //     return Boxes.All(x => x.OwnerId != _player.Id);
        // });
        
        private IObservable<bool> CanGoBack => Observable.Return(true);
        
        [ReactiveCommand(CanExecute = nameof(CanGoBack))]
        private void GoBack()
        {
            HostScreen.Router.NavigateBack.Execute();
        }
        private void InitializeBoxes() //TODO: Get from server
        {
            for (int i = 0; i < 5; i++)
            {
                var box = new BoxViewModel(Id, i);
                Boxes.Add(box);
            }
        }
    }
}
