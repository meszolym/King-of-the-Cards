using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.App.Frontend.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace KC.App.Frontend.Views;

partial class SessionView : ReactiveUserControl<SessionViewModel>
{
    public SessionView()
    {
        this.WhenActivated(d =>
        {
            this.BindCommand(this.ViewModel, vm => vm.GoBackCommand, v => v.NavbackButton).DisposeWith(d);
        });
        InitializeComponent();

    }
}