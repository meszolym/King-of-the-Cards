using KC.Frontend.Client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
using Splat;
namespace KC.Frontend.Client.ViewModels
{
    partial class MenuViewModel : ReactiveObject, IRoutableViewModel
    {
        private IObservable<bool> _joinCanExecute;
        private readonly ExternalCommunicatorService _externalCommunicator;
        
        [Reactive]
        private bool _sessionGetErrored = true;
        public MenuViewModel(IScreen host)
        {
            HostScreen = host;
            _joinCanExecute = this.WhenAnyValue(x => x.SelectedItem).Select(x => x != null!);
            
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();
        }

        [Reactive]
        //Can't be null, but compiler gives warning :)
        private SessionListItem _selectedItem = null!;

        [ReactiveCommand(CanExecute = nameof(_joinCanExecute))]
        private async Task JoinSession()
        {
            Debug.WriteLine("Joining session");
            await _externalCommunicator.JoinSession(SelectedItem.Id, ClientMacAddressHandler.PrimaryMacAddress);
            var session = await _externalCommunicator.GetSession(SelectedItem.Id);
            HostScreen.Router.Navigate.Execute(new SessionViewModel(HostScreen, session));
        }

        [ReactiveCommand]
        private void CreateSession()
        {
            Debug.WriteLine("Creating session");
        }
        
        [ReactiveCommand]
        private async Task TryGetSessions()
        {
            try
            {
                Sessions = (await _externalCommunicator.GetSessionList()).ToList();
                SessionGetErrored = false;
            }
            catch (Exception e)
            {
                SessionGetErrored = true;
            }
        }

        [Reactive]
        private List<SessionListItem> _sessions = new List<SessionListItem>();

        public string? UrlPathSegment { get; } = "menu";

        public IScreen HostScreen { get; }
    }
}
