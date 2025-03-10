using System;
using System.Diagnostics;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

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
            // ViewModel.LoadSessionsCommand.ThrownExceptions.Subscribe<Exception>(x =>
            // {
            //     Debug.WriteLine(x.Message);
            // });
            await ViewModel.LoadSessionsCommand.Execute().ObserveOn(RxApp.MainThreadScheduler);

        });
        InitializeComponent();
    }
}