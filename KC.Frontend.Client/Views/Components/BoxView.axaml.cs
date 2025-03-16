using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;

namespace KC.Frontend.Client.Views.Components;

public partial class BoxView : ReactiveUserControl<BoxViewModel>
{
    public BoxView()
    {
        ViewModel = new BoxViewModel();
        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.LeftHandViewModel, v => v.LeftHandView.ViewModel).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.RightHandViewModel, v => v.RightHandView.ViewModel).DisposeWith(d);
        });
        InitializeComponent();
    }
}