using System;
using System.Diagnostics;
using System.Linq;
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
        InitializeComponent();
        this.WhenActivated(async d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Sessions, v => v.SessionsListBox.ItemsSource).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.JoinSessionCommand, v => v.JoinSessionButton).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.CreateSessionCommand, v => v.CreateSessionButton).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedItemViewModel, v => v.SessionsListBox.SelectedItem).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.SessionGetErrored, v => v.NoConnStackPanel.IsVisible).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.NoSessions, v => v.NoSessionsTextBorder.IsVisible).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.TryGetSessionsCommand, v => v.GetSessionsTryAgainButton).DisposeWith(d);
            if (!Design.IsDesignMode)
            {
                await ViewModel!.TryGetSessionsCommand.Execute().ObserveOn(RxApp.MainThreadScheduler);
            }
            
        });
    }
}