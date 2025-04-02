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
    private ItemsControl _cardsItemsControl => this.FindControl<ItemsControl>("CardsItemsControl");
    public DealerView()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Cards, v => v._cardsItemsControl.ItemsSource).DisposeWith(disposables);
        });
    }
}