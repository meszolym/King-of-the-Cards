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
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
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

        private readonly ExternalCommunicatorService _externalCommunicator;
        private IObservable<bool> CanGoBack => BoxViewModel.BoxClaimStatusChanged.Select(_ =>
        {
            return Boxes.All(x => x.OwnerId != _player.Id);
        });
        
        [ReactiveCommand(CanExecute = nameof(CanGoBack))]
        private async Task NavBack()
        {
            await _externalCommunicator.LeaveSession(Id, ClientMacAddressHandler.PrimaryMacAddress);
            HostScreen.Router.NavigateBack.Execute();
        } 
        
        public SessionViewModel(IScreen hostScreen, Guid id)
        {
            this.HostScreen = hostScreen;
            Id = id;
            Boxes = new ObservableCollection<BoxViewModel>();
            Dealer = new DealerViewModel();
            InitializeBoxes();
            _player = Locator.Current.GetRequiredService<PlayerViewModel>();
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
        }

        public Guid Id { get; set; }

        public string? UrlPathSegment { get; } = "session";

        public IScreen HostScreen { get; }

        private PlayerViewModel _player;


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
