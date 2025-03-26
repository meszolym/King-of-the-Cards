using KC.Frontend.Client.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Frontend.Client.ViewModels.Components;

namespace KC.Frontend.Client.ViewModels
{
    partial class SessionViewModel : ReactiveObject, IRoutableViewModel
    {
        private SessionListItem selectedItem;
        private BoxViewModel _userControlledBox;
        [Reactive]
        private ObservableCollection<BoxViewModel> _boxes;

        [ReactiveCommand]
        private void TestSplit()
        {
            _userControlledBox.SplitHands();
        }
        public SessionViewModel(IScreen hostScreen, SessionListItem selectedItem)
        {
            this.HostScreen = hostScreen;
            this.selectedItem = selectedItem;
            Boxes = new ObservableCollection<BoxViewModel>();
            InitializeBoxes();
        }

        public string? UrlPathSegment { get; } = "session";

        public IScreen HostScreen { get; }

        [ReactiveCommand]
        private void GoBack()
        {
            HostScreen.Router.NavigateBack.Execute();
        }
        private void InitializeBoxes()
        {
            // Create 5 boxes, one for each player
            for (int i = 0; i < 5; i++)
            {
                var box = new BoxViewModel();
                if (i == 2) // Middle box is player-controlled
                {
                    box.IsPlayerControlled = true;
                    _userControlledBox = box;
                }
                Boxes.Add(box);
            }
        }
    }
}
