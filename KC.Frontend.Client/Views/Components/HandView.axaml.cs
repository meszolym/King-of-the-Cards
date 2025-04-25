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
    public HandView()
    {
        InitializeComponent();
        //ViewModel = new HandViewModel(this.Height);

        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Cards, v => v.CardsItemsControl.ItemsSource).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.BetAmount, v => v.BetTextBlock.Text, bet => $"${bet}").DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.IsPartOfSplit, v => v.BetTextBlock.IsVisible).DisposeWith(d);
        });
    }
}