using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels.Components;
using ReactiveUI;

namespace KC.Frontend.Client.Views.Components;

public partial class DealerView : ReactiveUserControl<DealerViewModel>
{
    public DealerView()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Cards, v => v.CardsItemsControl.ItemsSource).DisposeWith(disposables);
        });
    }
}