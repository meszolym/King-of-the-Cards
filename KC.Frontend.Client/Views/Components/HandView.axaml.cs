using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;

namespace KC.Frontend.Client.Views.Components;

public partial class HandView : ReactiveUserControl<HandViewModel>
{
    public HandView()
    {
        ViewModel = new HandViewModel();
        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.CardImageWithRects, v => v.CardImageList.ItemsSource).DisposeWith(d);
        });
        InitializeComponent();
    }
    
    
}