// BoxView.axaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.ViewModels;
using KC.Frontend.Client.ViewModels.Components;
using Splat;

namespace KC.Frontend.Client.Views.Components
{
    public partial class BoxView : ReactiveUserControl<BoxViewModel>
    {
        private HandView LeftHandViewFound => this.FindControl<HandView>(nameof(LeftHandView))!;
        private HandView RightHandViewFound => this.FindControl<HandView>(nameof(RightHandView))!;
        private TextBlock PlayerNameTextBlockFound => this.FindControl<TextBlock>(nameof(PlayerNameTextBlock))!;
        private Button ClaimButtonFound => this.FindControl<Button>(nameof(ClaimBoxButton))!;
        private Button UnclaimButtonFound => this.FindControl<Button>(nameof(UnclaimBoxButton))!;
        
        public BoxView()
        {
            InitializeComponent();
            //ViewModel = new BoxViewModel();
            
            this.WhenActivated(d =>
            {
                ViewModel.WhenAnyValue(vm => vm.IsSplit).Subscribe(UpdateColumnDefinitions).DisposeWith(d);
                // Bind the left hand view model
                this.OneWayBind(ViewModel, 
                    vm => vm.LeftHand, 
                    view => view.LeftHandViewFound.ViewModel)
                    .DisposeWith(d);
                //_rightHandView.IsVisible = false;
                this.OneWayBind(ViewModel,
                    vm => vm.IsSplit,
                    v => v.RightHandViewFound.IsVisible);
                // Bind the right hand view model
                this.OneWayBind(ViewModel, 
                    vm => vm.RightHand, 
                    view => view.RightHandViewFound.ViewModel)
                    .DisposeWith(d);

                // Bind player name
                this.Bind(ViewModel, 
                        vm => vm.PlayerName, 
                        v => v.PlayerNameTextBlockFound.Text)
                    .DisposeWith(d);
                
                // // Update appearance when player controlled
                // this.WhenAnyValue(x => x.ViewModel.IsPlayerControlled)
                //     .Subscribe(isPlayerControlled => UpdatePlayerControlledState(isPlayerControlled))
                //     .DisposeWith(d);
                
                // Update visibility of right hand based on split state
                this.WhenAnyValue(x => x.ViewModel.IsSplit)
                    .Subscribe(isSplit => RightHandViewFound.IsVisible = isSplit)
                    .DisposeWith(d);

                var localPlayerGuid = Locator.Current.GetRequiredService<PlayerViewModel>().Id;
                
                this.BindCommand(ViewModel, vm => vm.ClaimBoxCommand, view => view.ClaimButtonFound);
                this.OneWayBind(ViewModel, vm => vm.IsClaimed, v=> v.ClaimButtonFound.IsVisible, b => !b && ViewModel.OwnerId == Guid.Empty);
                this.BindCommand(ViewModel, vm => vm.UnclaimBoxCommand, view => view.UnclaimButtonFound);
                this.OneWayBind(ViewModel, vm => vm.IsClaimed, v=> v.UnclaimButtonFound.IsVisible, b => b && ViewModel.OwnerId == localPlayerGuid);
                
                

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
        
        // private void UpdatePlayerControlledState(bool isPlayerControlled)
        // {
        //     if (isPlayerControlled)
        //     {
        //         // Add visual indication that this box is player-controlled
        //         // For example, change border color or add a highlight effect
        //         var border = this.Parent as Border;
        //         if (border != null)
        //         {
        //             border.BorderBrush = new Avalonia.Media.SolidColorBrush(
        //                 Avalonia.Media.Color.FromRgb(255, 215, 0)); // Gold color
        //             border.BorderThickness = new Thickness(3);
        //         }
        //     }
        // }
    }
}