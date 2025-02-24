using Avalonia.Controls;
using Avalonia.ReactiveUI;
using KC.App.Frontend.ViewModels;
using ReactiveUI;

namespace KC.App.Frontend.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    //private ListBox SessionsListBox => this.FindControl<ListBox>("SessionsListBox");

    public MainWindow()
    {
        ViewModel = new MainWindowViewModel();
        this.WhenActivated(d =>
        {
            //SessionsListBox.ItemsSource = ViewModel.Sessions;
        });
        InitializeComponent();
    }

}