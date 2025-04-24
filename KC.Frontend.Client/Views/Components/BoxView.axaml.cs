// BoxView.axaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Runtime.InteropServices.ComTypes;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.ViewModels;
using KC.Frontend.Client.ViewModels.Components;
using Splat;

namespace KC.Frontend.Client.Views.Components
{
    public partial class BoxView : ReactiveUserControl<BoxViewModel>
    {
        public BoxView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                // Bind the right hand view model
                this.OneWayBind(ViewModel, 
                        vm => vm.RightHand, 
                        view => view.RightHandView.ViewModel)
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TurnInfo, v => v.TurnIndicatorRightImage.IsVisible, t => t == BoxViewModel.TurnState.Right).DisposeWith(d);
                
                // Bind the left hand view model
                this.OneWayBind(ViewModel, 
                    vm => vm.LeftHand, 
                    view => view.LeftHandView.ViewModel)
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TurnInfo, v => v.TurnIndicatorLeftImage.IsVisible, t => t == BoxViewModel.TurnState.Left).DisposeWith(d);
                
                // Bind player name
                this.Bind(ViewModel, 
                        vm => vm.PlayerName, 
                        v => v.PlayerNameTextBlock.Text)
                    .DisposeWith(d);
                
                
                this.BindCommand(ViewModel, vm => vm.ClaimBoxCommand, v => v.ClaimBoxButton);
                this.OneWayBind(ViewModel, vm => vm.OwnerId, v=> v.ClaimBoxButton.IsVisible, g => g == Guid.Empty);
                this.BindCommand(ViewModel, vm => vm.DisclaimBoxCommand, v => v.DisclaimBoxButton);
                this.OneWayBind(ViewModel, vm => vm.OwnerId, v=> v.DisclaimBoxButton.IsVisible, g => g == ViewModel!.LocalPlayer.Id);
                
                this.OneWayBind(ViewModel, vm => vm.RightHand.BetAmount, v => v.BetTextBlock.Text, bet => $"${bet}").DisposeWith(d);

                //I hate avalonia for this
                BetTextBlock.Bind(TextBlock.IsVisibleProperty, ViewModel.IsBettingTextVisible).DisposeWith(d);
                BetNumericUpDown.Bind(NumericUpDown.IsVisibleProperty, ViewModel.IsBettingModifierVisible).DisposeWith(d);
                
                this.Bind(ViewModel, vm => vm.RightHand.BetAmount, v => v.BetNumericUpDown.Value).DisposeWith(d);
                BetNumericUpDown.ValueChanged += async (sender, args) =>
                {
                    if (!BetNumericUpDown.IsVisible) return;
                    var succ = await ViewModel.UpdateBetAmount(args.OldValue, args.NewValue);
                    if (!succ)
                        BetNumericUpDown.Value = args.OldValue;
                };
                

            });
        }
    }
}