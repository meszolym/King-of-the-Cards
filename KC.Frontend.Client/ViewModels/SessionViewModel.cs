using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
using KC.Frontend.Client.ViewModels.Components;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using Splat;

namespace KC.Frontend.Client.ViewModels
{
    partial class SessionViewModel : ReactiveObject, IRoutableViewModel
    {
        
        [Reactive]
        private ObservableCollection<BoxViewModel> _boxes;
        
        [Reactive]
        private DealerViewModel _dealer;

        private readonly ExternalCommunicatorService _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();

        private IObservable<bool> CanGoBack => BoxViewModel.BoxClaimStatusChanged.Select(_ =>
        {
            return Boxes.All(x => x.OwnerId != _player.Id);
        }).StartWith(Boxes.All(x => x.OwnerId != _player.Id));

        [ReactiveCommand(CanExecute = nameof(CanGoBack))]
        private async Task NavBack()
        {
            await _externalCommunicator.LeaveSession(Id, ClientMacAddressHandler.PrimaryMacAddress);
            await HostScreen.Router.NavigateBack.Execute();
        }

        [Reactive]
        private bool _bettingPhase;

        [Reactive]
        private string _bettingTimeLeft = "Waiting for first bet...";
        
        public SessionViewModel(IScreen hostScreen, SessionReadDto session)
        {
            this.HostScreen = hostScreen;
            Id = session.Id;
            BettingPhase = session.CanPlaceBets;
            Boxes = new ObservableCollection<BoxViewModel>(session.Table.BettingBoxes.Select(x => new BoxViewModel(Id, x, session.CanPlaceBets)));
            Dealer = new DealerViewModel(session.Table.DealerVisibleCards);
            
            ExternalCommunicatorService.SignalREvents.BettingTimerTicked.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(dto => BettingTimeLeft = $"Time left: {dto.remainingSeconds} seconds");

            ExternalCommunicatorService.SignalREvents.BettingTimerStopped.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => BettingTimeLeft = "Waiting for first bet...");
            
            ExternalCommunicatorService.SignalREvents.BettingTimerElapsed.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => BettingPhase = false);
            
            
        }

        [Reactive]
        private Guid _id;

        public string? UrlPathSegment { get; } = "session";

        public IScreen HostScreen { get; }

        private readonly PlayerViewModel _player = Locator.Current.GetRequiredService<PlayerViewModel>();
    }
}
