using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using KC.Frontend.Client.ViewModels.Components;
using ReactiveUI;

namespace KC.Frontend.Client.Views.Components;

public partial class SessionBoxControlsView : ReactiveUserControl<SessionBoxControlsViewModel>
{
    public SessionBoxControlsView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.HitOnHandCommand, v => v.HitButton).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.StandOnHandCommand, v => v.StandButton).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.DoubleDownOnHandCommand, v => v.DoubleButton).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.SplitOnHandCommand, v => v.SplitButton).DisposeWith(d);
        });
        
    }
}