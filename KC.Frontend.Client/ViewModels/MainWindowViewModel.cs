using System;
using System.Diagnostics;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
using Microsoft.AspNetCore.Connections.Features;
using Splat;

namespace KC.Frontend.Client.ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject, IScreen
    {
        [Reactive]
        private bool _isRegistered;
        
        [Reactive]
        private bool _isConnected = true;

        [Reactive] 
        private bool _isFullScreen = true;
        
        private readonly ExternalCommunicatorService _externalCommunicator;
        
        [Reactive]
        private PlayerViewModel _playerViewModel;

        public MainWindowViewModel()
        {
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
            PlayerViewModel = Locator.Current.GetRequiredService<PlayerViewModel>();
            _externalCommunicator.ConnectionStatus.ObserveOn(RxApp.MainThreadScheduler).Subscribe(b => IsConnected = b);
            ClientMacAddress = ClientMacAddressHandler.PrimaryMacAddress.ToString();
            
            Router.NavigationChanged.ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ => IsFullScreen = Router.GetCurrentViewModel() is SessionViewModel);
            
            ExternalCommunicatorService.SignalREvents.PlayerBalanceUpdated.ObserveOn(RxApp.MainThreadScheduler).Subscribe(player => PlayerViewModel.PlayerBalance = player.Balance);
        }
        
        [Reactive]
        private string _clientMacAddress;
        
        public async Task InitAsync()
        {
            while (!_externalCommunicator.SignalRInitialized)
            {
                try
                {
                    await _externalCommunicator.ConnectToSignalR();
                }
                catch (Exception e)
                {
                    await Task.Delay(3000);
                }
            }
            
            while (!_isRegistered)
            {
                await Register();
            }
            
            await _externalCommunicator.UpdatePlayerConnectionId(ClientMacAddressHandler.PrimaryMacAddress);
            await Router.Navigate.Execute(new MenuViewModel(this));
        }
        
        public Interaction<string?, string?> PlayerNameInteraction { get; } = new Interaction<string?, string?>();
        
        private async Task Register()
        {
            if (!_isConnected)
            {
                throw new InvalidOperationException("Cannot register when not connected");
            }
            await CheckIfRegistered();
            if (_isRegistered)
                return;
            var name = await PlayerNameInteraction.Handle(null);
            try
            {
                await _externalCommunicator.RegisterPlayer(name!);
                await CheckIfRegistered();
            }
            catch (Exception e)
            {
                //TODO: Show dialog
                Debug.WriteLine(e.Message + "at Register in MainWindowViewModel");
            }
            
        }

        private async Task CheckIfRegistered()
        {
            try
            {
                var player = await _externalCommunicator.GetLocalPlayer();
                PlayerViewModel.Id = player.Id;
                PlayerViewModel.PlayerName = player.Name;
                PlayerViewModel.PlayerBalance = player.Balance;
                _isRegistered = true;
            }
            catch (Exception e)
            {
                _isRegistered = false;
                //TODO: Show dialog
                Debug.WriteLine(e.Message + "at CheckIfRegistered in MainWindowViewModel");
            }
        }
        public RoutingState Router { get; } = new();

        [ReactiveCommand]
        private async Task ResetBalance() => _externalCommunicator.ResetPlayerBalance();
    }
}
