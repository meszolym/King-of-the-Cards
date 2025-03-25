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
            this.WhenAnyValue(x => x.ViewModel.IsActive)
                .Subscribe(isActive => UpdateActiveState(isActive))
                .DisposeWith(disposables);
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateActiveState(bool isActive)
    {
        // Change appearance when active
        // Background = isActive
        //     ? new SolidColorBrush(Color.FromRgb(70, 130, 180))
        //     : new SolidColorBrush(Color.FromRgb(30, 61, 89));
    }
}