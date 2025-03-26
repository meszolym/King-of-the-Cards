using KC.Frontend.Client.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using Splat;

namespace KC.Frontend.Client.ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject, IScreen
    {
        [Reactive]
        private bool _isRegistered = false;
        
        [Reactive]
        private bool _isConnected = false;
        
        private readonly ExternalCommunicatorService _externalCommunicator;
        public MainWindowViewModel()
        {
            Router.Navigate.Execute(new MenuViewModel(this));
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
            _externalCommunicator.ConnectionStatus.ObserveOn(RxApp.MainThreadScheduler).Subscribe(b => IsConnected = b);
        }

        [ReactiveCommand]
        private async Task Register()
        {
            
        }
        public RoutingState Router { get; } = new();
    }
}
