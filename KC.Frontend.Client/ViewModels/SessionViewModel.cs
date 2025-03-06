using KC.Frontend.Client.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Frontend.Client.ViewModels
{
    partial class SessionViewModel : ReactiveObject, IRoutableViewModel
    {
        private SessionListItem selectedItem;

        public SessionViewModel(IScreen hostScreen, SessionListItem selectedItem)
        {
            this.HostScreen = hostScreen;
            this.selectedItem = selectedItem;
        }

        public string? UrlPathSegment { get; } = "session";

        public IScreen HostScreen { get; }

        [ReactiveCommand]
        private void GoBack()
        {
            HostScreen.Router.NavigateBack.Execute();
        }
    }
}
