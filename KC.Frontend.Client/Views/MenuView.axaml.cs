using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace KC.Frontend.Client.Views;

partial class MenuView : ReactiveUserControl<MenuViewModel>
{
    public MenuView()
    {
        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Sessions, v => v.SessionsListBox.ItemsSource).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.JoinSessionCommand, v => v.JoinSessionButton).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.CreateSessionCommand, v => v.CreateSessionButton).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedItem, v => v.SessionsListBox.SelectedItem).DisposeWith(d);
        });
        InitializeComponent();
    }
}