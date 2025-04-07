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
        private WindowState _windowState = WindowState.Maximized;
        
        private readonly ExternalCommunicatorService _externalCommunicator;
        
        public readonly PlayerViewModel PlayerViewModel;
        public MainWindowViewModel()
        {
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
            PlayerViewModel = Locator.Current.GetRequiredService<PlayerViewModel>();
            _externalCommunicator.ConnectionStatus.ObserveOn(RxApp.MainThreadScheduler).Subscribe(b => IsConnected = b);
            ClientMacAddress = ClientMacAddressHandler.PrimaryMacAddress.ToString();
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
                    //TODO: Show dialog
                    await Task.Delay(3000);
                }
            }

            await Register();
            await _externalCommunicator.UpdatePlayerConnectionId();
            Router.Navigate.Execute(new MenuViewModel(this));
        }
        
        public Interaction<string?, string?> PlayerNameInteraction { get; } = new Interaction<string?, string?>();
        
        //[ReactiveCommand]
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
            }
            
        }

        private async Task CheckIfRegistered()
        {
            try
            {
                var player = await _externalCommunicator.GetPlayerByMac(ClientMacAddressHandler.PrimaryMacAddress);
                PlayerViewModel.PlayerName = player.Name;
                PlayerViewModel.PlayerBalance = player.Balance;
                _isRegistered = true;
            }
            catch (Exception e)
            {
                _isRegistered = false;
                //TODO: Show dialog
            }
        }
        public RoutingState Router { get; } = new();
    }
}
