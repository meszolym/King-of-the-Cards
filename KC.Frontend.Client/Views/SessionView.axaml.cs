using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using KC.Frontend.Client.ViewModels.Components;

namespace KC.Frontend.Client.Views;

partial class SessionView : ReactiveUserControl<SessionViewModel>
{
    // private Button _testSplitButton => this.FindControl<Button>("TestSplitButton");
    public SessionView()
    {
        this.WhenActivated(d =>
        {
            // this.BindCommand(ViewModel, vm => vm.TestSplitCommand, v => v._testSplitButton).DisposeWith(d);
          // Observer.Create<double>(d => BoxesItemsControl.Width = d);
            // this.GetObservable(WidthProperty).Subscribe(Observer.Create<double>(d =>
            // {
            //     BoxesItemsControl.Width = d;
            // })).DisposeWith(d);
            // this.Bind(WidthProperty, this.GetObservable(WidthProperty), BoxesItemsControl, ItemsControl.WidthProperty).DisposeWith(d);
            //this.BindCommand(this.ViewModel, vm => vm.GoBackCommand, v => v.NavbackButton).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.Boxes, v => v.BoxesItemsControl.ItemsSource).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.Dealer, v => v.DealerView.ViewModel).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.NavBackCommand, v => v.NavBackButton).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.Id, v => v.SessionIdTextBlock.Text, i => $"Session ID: {i}").DisposeWith(d);
        });
        InitializeComponent();

    }
}