// BoxView.axaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using KC.Frontend.Client.ViewModels.Components;

namespace KC.Frontend.Client.Views.Components
{
    public partial class BoxView : ReactiveUserControl<BoxViewModel>
    {
        private HandView _leftHandView;
        private HandView _rightHandView;
        
        public BoxView()
        {
            InitializeComponent();
            ViewModel = new BoxViewModel();
            _leftHandView = this.FindControl<HandView>("LeftHandView");
            _rightHandView = this.FindControl<HandView>("RightHandView");
            
            this.WhenActivated(disposables =>
            {
                ViewModel.WhenAnyValue(vm => vm.IsSplit).Subscribe(UpdateColumnDefinitions).DisposeWith(disposables);
                // Bind the left hand view model
                this.OneWayBind(ViewModel, 
                    vm => vm.LeftHand, 
                    view => view._leftHandView.ViewModel)
                    .DisposeWith(disposables);
                //_rightHandView.IsVisible = false;
                this.OneWayBind(ViewModel,
                    vm => vm.IsSplit,
                    v => v._rightHandView.IsVisible);
                // Bind the right hand view model
                this.OneWayBind(ViewModel, 
                    vm => vm.RightHand, 
                    view => view._rightHandView.ViewModel)
                    .DisposeWith(disposables);
                
                // Update appearance when player controlled
                this.WhenAnyValue(x => x.ViewModel.IsPlayerControlled)
                    .Subscribe(isPlayerControlled => UpdatePlayerControlledState(isPlayerControlled))
                    .DisposeWith(disposables);
                
                // Update visibility of right hand based on split state
                this.WhenAnyValue(x => x.ViewModel.IsSplit)
                    .Subscribe(isSplit => _rightHandView.IsVisible = isSplit)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void UpdateColumnDefinitions(bool isSplit)
        {
            var grid = this.FindControl<Grid>("MainGrid"); // Add x:Name="MainGrid" to your Grid in XAML
    
            if (isSplit)
            {
                // Split evenly when right hand is visible
                grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                // Left hand takes all space when right hand is hidden
                grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions[1].Width = GridLength.Auto;
            }
        }
        private void UpdatePlayerControlledState(bool isPlayerControlled)
        {
            if (isPlayerControlled)
            {
                // Add visual indication that this box is player-controlled
                // For example, change border color or add a highlight effect
                var border = this.Parent as Border;
                if (border != null)
                {
                    border.BorderBrush = new Avalonia.Media.SolidColorBrush(
                        Avalonia.Media.Color.FromRgb(255, 215, 0)); // Gold color
                    border.BorderThickness = new Thickness(3);
                }
            }
        }
    }
}