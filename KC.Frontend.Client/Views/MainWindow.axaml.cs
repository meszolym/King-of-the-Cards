using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using KC.Frontend.Client.ViewModels.Dialogs;

namespace KC.Frontend.Client.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(async d =>
        {
            this.OneWayBind(ViewModel, vm=> vm.IsFullScreen, v=> v.WindowState, b => b ? WindowState.Maximized : WindowState.Normal).DisposeWith(d);
            this.OneWayBind(ViewModel, 
                    vm => vm.IsFullScreen, 
                    v => v.SystemDecorations, 
                    b => b ? SystemDecorations.None : SystemDecorations.Full)
                .DisposeWith(d);
            this.Bind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.IsConnected, v => v.RoutedViewHost.IsEnabled).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.PlayerNameInteraction, HandleNameChange).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.PlayerViewModel.PlayerName, v => v.PlayerNameTextBlock.Text, n => $"Name: {n}").DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.PlayerViewModel.PlayerBalance, v => v.PlayerBalanceTextBlock.Text, bt => $"Balance: ${bt}" ).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.ClientMacAddress, v => v.PlayerMacTextBlock.Text, mac => $"(MAC: {mac})").DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.ResetBalanceCommand, v => v.ResetBalanceButton).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.ResetBalanceEnabled, v => v.ResetBalanceButton.IsVisible).DisposeWith(d);
            
            this.BeforeFirstConnStackPanel.IsVisible = true;
            await ViewModel!.InitAsync();
            this.BeforeFirstConnStackPanel.IsVisible = false;
        });
        
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