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
using Constants = KC.Shared.Models.Misc.Constants;

namespace KC.Frontend.Client.ViewModels
{
    public partial class SessionViewModel : ReactiveObject, IRoutableViewModel
    {
        
        [Reactive]
        private ObservableCollection<BoxViewModel> _boxes;
        
        [Reactive]
        private DealerViewModel _dealer;

        [Reactive]
        private SessionBoxControlsViewModel _controls;
        
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

        [Reactive] private bool _shuffling;
        
        public SessionViewModel(IScreen hostScreen, SessionReadDto session)
        {
            this.HostScreen = hostScreen;
            Id = session.Id;
            BettingPhase = session.CanPlaceBets;
            Boxes = new ObservableCollection<BoxViewModel>(session.Table.BettingBoxes.OrderByDescending(b => b.BoxIdx).Select(b => new BoxViewModel(Id, b, session.CurrentTurnInfo, session.CanPlaceBets)));
            Dealer = new DealerViewModel(session.Table.DealerVisibleCards);
            
            Controls = new SessionBoxControlsViewModel(this);
            
            ExternalCommunicatorService.SignalREvents.BettingTimerTicked.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(dto => BettingTimeLeft = $"Time left: {dto.remainingSeconds} seconds");

            ExternalCommunicatorService.SignalREvents.BettingTimerStopped.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => BettingTimeLeft = "Waiting for first bet...");

            ExternalCommunicatorService.SignalREvents.BettingTimerElapsed.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => BettingPhase = false);
            
            ExternalCommunicatorService.SignalREvents.BettingReset.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(dto =>
                {
                    BettingPhase = dto.CanPlaceBets;
                    BettingTimeLeft = "Waiting for first bet...";
                });

            ExternalCommunicatorService.SignalREvents.Shuffling.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe( async guid =>
                {
                    Shuffling = true;
                    await Task.Delay(Constants.ShufflingDelayMs);
                    Shuffling = false;
                });
        }

        [Reactive]
        private Guid _id;

        public string? UrlPathSegment { get; } = "session";

        public IScreen HostScreen { get; }

        private readonly PlayerViewModel _player = Locator.Current.GetRequiredService<PlayerViewModel>();
    }
}
