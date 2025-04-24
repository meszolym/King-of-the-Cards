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
        private HandView LeftHandViewFound => this.FindControl<HandView>(nameof(LeftHandView))!;
        private HandView RightHandViewFound => this.FindControl<HandView>(nameof(RightHandView))!;
        private TextBlock PlayerNameTextBlockFound => this.FindControl<TextBlock>(nameof(PlayerNameTextBlock))!;
        private Button ClaimButtonFound => this.FindControl<Button>(nameof(ClaimBoxButton))!;
        private Button UnclaimButtonFound => this.FindControl<Button>(nameof(UnclaimBoxButton))!;
        private TextBlock BetTextBlockFound => this.FindControl<TextBlock>(nameof(BetTextBlock))!;
        private NumericUpDown BetNumericUpDownFound => this.FindControl<NumericUpDown>(nameof(BetNumericUpDown))!;
        public BoxView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                // Bind the right hand view model
                this.OneWayBind(ViewModel, 
                        vm => vm.RightHand, 
                        view => view.RightHandViewFound.ViewModel)
                    .DisposeWith(d);

                
                // Bind the left hand view model
                this.OneWayBind(ViewModel, 
                    vm => vm.LeftHand, 
                    view => view.LeftHandViewFound.ViewModel)
                    .DisposeWith(d);

                // Bind player name
                this.Bind(ViewModel, 
                        vm => vm.PlayerName, 
                        v => v.PlayerNameTextBlockFound.Text)
                    .DisposeWith(d);
                
                
                this.BindCommand(ViewModel, vm => vm.ClaimBoxCommand, view => view.ClaimButtonFound);
                this.OneWayBind(ViewModel, vm => vm.OwnerId, v=> v.ClaimButtonFound.IsVisible, g => g == Guid.Empty);
                this.BindCommand(ViewModel, vm => vm.DisclaimBoxCommand, view => view.UnclaimButtonFound);
                this.OneWayBind(ViewModel, vm => vm.OwnerId, v=> v.UnclaimButtonFound.IsVisible, g => g == ViewModel!.LocalPlayer.Id);
                
                
                this.OneWayBind(ViewModel, vm => vm.RightHand.BetAmount, v => v.BetTextBlockFound.Text, bet => $"${bet}").DisposeWith(d);

                //I hate avalonia for this
                BetTextBlockFound.Bind(TextBlock.IsVisibleProperty, ViewModel.IsBettingTextVisible).DisposeWith(d);
                BetNumericUpDownFound.Bind(NumericUpDown.IsVisibleProperty, ViewModel.IsBettingModifierVisible).DisposeWith(d);
                
                this.Bind(ViewModel, vm => vm.RightHand.BetAmount, v => v.BetNumericUpDownFound.Value).DisposeWith(d);
                BetNumericUpDownFound.ValueChanged += async (sender, args) =>
                {
                    if (!BetNumericUpDownFound.IsVisible) return;
                    var succ = await ViewModel.UpdateBetAmount(args.OldValue, args.NewValue);
                    if (!succ)
                        BetNumericUpDownFound.Value = args.OldValue;
                };
                

            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}