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
    private readonly ItemsControl _cardsItemsControl;
    //private const double CardOffsetX = 20; // Horizontal offset for each card

    public HandView()
    {
        InitializeComponent();
        ViewModel = new HandViewModel();
        _cardsItemsControl = this.FindControl<ItemsControl>("CardsItemsControl");
        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Cards, v =>v._cardsItemsControl.ItemsSource).DisposeWith(disposables);
            this.WhenAnyValue(x => x.ViewModel.Cards)
                .Subscribe(_ => UpdateCardPositions())
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ViewModel.IsActive)
                .Subscribe(isActive => UpdateActiveState(isActive))
                .DisposeWith(disposables);

            // Subscribe to collection changes
            ViewModel.WhenAnyValue(vm => vm.Cards).Subscribe(_ => UpdateCardPositions()).DisposeWith(disposables);
            // this.WhenAnyValue(x => x.ViewModel.Cards)
            //     .Where(cards => cards != null)
            //     .Select(cards => cards as INotifyCollectionChanged)
            //     .Subscribe(observable => { observable.CollectionChanged += Cards_CollectionChanged; })
            //     .DisposeWith(disposables);
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Cards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateCardPositions();
    }

    private const double CardOffsetX = 25; // Horizontal offset for each card
    private const double CardOffsetY = -25; // Negative vertical offset to move cards upward

    private void UpdateCardPositions()
    {
        if (_cardsItemsControl == null || _cardsItemsControl.Items == null) return;

        var itemCount = _cardsItemsControl.Items.Cast<object>().Count();
    
        // Reverse the loop to set proper Z-index (first card on top)
        for (var i = 0; i < itemCount; i++)
        {
            var container = _cardsItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
            if (container != null)
            {
                // Position cards starting from bottom left
                Canvas.SetLeft(container, i * CardOffsetX);
                Canvas.SetBottom(container, i * -CardOffsetY); // Use bottom instead of top
            
                // First card (index 0) should have highest Z-index
                container.ZIndex = i; //itemCount - i;
            }
        }
    }

    private void UpdateActiveState(bool isActive)
    {
        // Change appearance when active
        Background = isActive
            ? new SolidColorBrush(Color.FromRgb(70, 130, 180))
            : new SolidColorBrush(Color.FromRgb(30, 61, 89));
    }
}