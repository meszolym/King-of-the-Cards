using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using KC.Frontend.Client.ViewModels.Components;
using ReactiveUI;

namespace KC.Frontend.Client.Views.Components;

public partial class HandView : ReactiveUserControl<HandViewModel>
{
    //private const double CardOffsetX = 20; // Horizontal offset for each card
    private ItemsControl _cardsItemsControl => this.FindControl<ItemsControl>("CardsItemsControl");
    public HandView()
    {
        InitializeComponent();
        //ViewModel = new HandViewModel(this.Height);

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Cards, v => v._cardsItemsControl.ItemsSource).DisposeWith(disposables);
        });
        
        //TODO: Bet display + bet setting
        //TODO: Display something to show that hand is in turn
    }
}