using KC.Frontend.Client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;

namespace KC.Frontend.Client.ViewModels
{
    partial class MenuViewModel : ReactiveObject, IRoutableViewModel
    {
        private IObservable<bool> _joinCanExecute;
        public MenuViewModel(IScreen host)
        {
            HostScreen = host;
            _joinCanExecute = this.WhenAnyValue(x => x.SelectedItem).Select(x => x != null);
            Sessions.Add(new()
            {
                Id = System.Guid.NewGuid()
            });
        }

        [Reactive]
        private SessionListItem _selectedItem;

        [ReactiveCommand(CanExecute = nameof(_joinCanExecute))]
        private void JoinSession()
        {
            Debug.WriteLine("Joining session");
            HostScreen.Router.Navigate.Execute(new SessionViewModel(HostScreen, SelectedItem));
        }

        [ReactiveCommand]
        private void CreateSession()
        {
            Debug.WriteLine("Creating session");
        }

        public List<SessionListItem> Sessions = new List<SessionListItem>();

        public string? UrlPathSegment { get; } = "menu";

        public IScreen HostScreen { get; }
    }
}
