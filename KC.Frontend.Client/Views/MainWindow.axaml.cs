using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KC.Frontend.Client.ViewModels.Dialogs;

namespace KC.Frontend.Client.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.IsConnected, v => v.RoutedViewHost.IsEnabled).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.PlayerNameInteraction, HandleNameChange).DisposeWith(d);
            ViewModel!.RegisterCommand.Execute().ObserveOn(RxApp.MainThreadScheduler); //TODO: Do on first connect to SignalR
        });
        InitializeComponent();
        
    }
    private async Task HandleNameChange(IInteractionContext<string?, string?> arg)
    {
        var view = new Dialogs.PlayerNameDialog()
        {
            ViewModel = new PlayerNameDialogViewModel(arg.Input)
        };
        var res = await view.ShowDialog<string?>(this);
        arg.SetOutput(res);
    }



}