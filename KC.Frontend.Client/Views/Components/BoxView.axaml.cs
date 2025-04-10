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

                var localPlayerGuid = Locator.Current.GetRequiredService<PlayerViewModel>().Id;
                
                this.BindCommand(ViewModel, vm => vm.ClaimBoxCommand, view => view.ClaimButtonFound);
                this.OneWayBind(ViewModel, vm => vm.IsClaimed, v=> v.ClaimButtonFound.IsVisible, b => !b && ViewModel.OwnerId == Guid.Empty);
                this.BindCommand(ViewModel, vm => vm.DisclaimBoxCommand, view => view.UnclaimButtonFound);
                this.OneWayBind(ViewModel, vm => vm.IsClaimed, v=> v.UnclaimButtonFound.IsVisible, b => b && ViewModel.OwnerId == localPlayerGuid);

            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}