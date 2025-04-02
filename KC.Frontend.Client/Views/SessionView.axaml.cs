using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace KC.Frontend.Client.Views;

partial class SessionView : ReactiveUserControl<SessionViewModel>
{
    private Button _testSplitButton => this.FindControl<Button>("TestSplitButton");
    public SessionView()
    {
        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.TestSplitCommand, v => v._testSplitButton).DisposeWith(d);
          // Observer.Create<double>(d => BoxesItemsControl.Width = d);
            // this.GetObservable(WidthProperty).Subscribe(Observer.Create<double>(d =>
            // {
            //     BoxesItemsControl.Width = d;
            // })).DisposeWith(d);
            // this.Bind(WidthProperty, this.GetObservable(WidthProperty), BoxesItemsControl, ItemsControl.WidthProperty).DisposeWith(d);
            //this.BindCommand(this.ViewModel, vm => vm.GoBackCommand, v => v.NavbackButton).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.Boxes, v => v.BoxesItemsControl.ItemsSource).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.Dealer, v => v.DealerView.ViewModel).DisposeWith(d);
        });
        InitializeComponent();

    }
}