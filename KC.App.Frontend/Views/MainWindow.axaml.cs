using Avalonia.Controls;
using Avalonia.ReactiveUI;
using KC.App.Frontend.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace KC.App.Frontend.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router).DisposeWith(d);
        });
        InitializeComponent();
    }



}