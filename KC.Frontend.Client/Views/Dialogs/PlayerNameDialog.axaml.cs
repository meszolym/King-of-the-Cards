using System;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using KC.Frontend.Client.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace KC.Frontend.Client.Views.Dialogs;

public partial class PlayerNameDialog : ReactiveWindow<PlayerNameDialogViewModel>
{
    public PlayerNameDialog()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.BindValidation(ViewModel, vm => vm.PlayerName, v => v.NewNameValidation.Text).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.IsRename, v => v.CancelButton.IsEnabled).DisposeWith(d);
            CancelButton.Click += (_, _) => Close(null);
            this.Bind(ViewModel, vm => vm.PlayerName, v => v.NewNameTextBox.Text).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
            ViewModel!.SaveCommand.Subscribe(_ => Close(ViewModel.PlayerName)).DisposeWith(d);
        });
    }
}