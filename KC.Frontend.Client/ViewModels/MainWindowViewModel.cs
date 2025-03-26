using System;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
using Splat;

namespace KC.Frontend.Client.ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject, IScreen
    {
        [Reactive]
        private bool _isRegistered;
        
        [Reactive]
        private bool _isConnected = true;
        
        private readonly ExternalCommunicatorService _externalCommunicator;
        public MainWindowViewModel()
        {
            Router.Navigate.Execute(new MenuViewModel(this));
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
            //_externalCommunicator.ConnectionStatus.ObserveOn(RxApp.MainThreadScheduler).Subscribe(b => IsConnected = b);
        }

        public Interaction<string?, string?> PlayerNameInteraction { get; } = new Interaction<string?, string?>();
        
        [ReactiveCommand]
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
                await _externalCommunicator.GetPlayerByMac(ClientMacAddressHandler.GetMacAddress());
                _isRegistered = true;
            }
            catch (Exception e)
            {
                _isRegistered = false;
            }
        }
        public RoutingState Router { get; } = new();
    }
}
