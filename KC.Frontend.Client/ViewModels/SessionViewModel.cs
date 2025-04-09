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
using KC.Shared.Models.Dtos;
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
        }).StartWith(true);
        
        [ReactiveCommand(CanExecute = nameof(CanGoBack))]
        private async Task NavBack()
        {
            await _externalCommunicator.LeaveSession(Id, ClientMacAddressHandler.PrimaryMacAddress);
            HostScreen.Router.NavigateBack.Execute();
        } 
        
        public SessionViewModel(IScreen hostScreen, SessionReadDto session)
        {
            this.HostScreen = hostScreen;
            Id = session.Id;
            Boxes = new(session.Table.BettingBoxes.Select(x => new BoxViewModel(Id, x)));
            Dealer = new DealerViewModel(session.Table.DealerVisibleCards);
            _player = Locator.Current.GetRequiredService<PlayerViewModel>();
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
        }

        public Guid Id { get; set; }

        public string? UrlPathSegment { get; } = "session";

        public IScreen HostScreen { get; }

        private PlayerViewModel _player;
    }
}
