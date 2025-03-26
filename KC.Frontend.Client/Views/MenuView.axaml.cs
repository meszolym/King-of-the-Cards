using System;
using System.Diagnostics;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;

namespace KC.Frontend.Client.Views;

partial class MenuView : ReactiveUserControl<MenuViewModel>
{
    public MenuView()
    {
        this.WhenActivated(async d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Sessions, v => v.SessionsListBox.ItemsSource).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.JoinSessionCommand, v => v.JoinSessionButton).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.CreateSessionCommand, v => v.CreateSessionButton).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedItem, v => v.SessionsListBox.SelectedItem).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.NoConnection, v => v.NoConnStackPanel.IsVisible).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.NoConnection, v => v.NoSessionsTextBorder.IsVisible,
                b => !b && ViewModel!.Sessions.Count == 0).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.TryConnCommand, v => v.ConnTryAgainButton).DisposeWith(d);
            if (!Design.IsDesignMode)
            {
                await ViewModel!.TryConnCommand.Execute().ObserveOn(RxApp.MainThreadScheduler);
            }
            
        });
        InitializeComponent();
    }
}