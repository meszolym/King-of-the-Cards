using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace KC.Frontend.Client.Views;

partial class SessionView : ReactiveUserControl<SessionViewModel>
{
    public SessionView()
    {
        this.WhenActivated(d =>
        {
            //this.BindCommand(this.ViewModel, vm => vm.GoBackCommand, v => v.NavbackButton).DisposeWith(d);
        });
        InitializeComponent();

    }
}