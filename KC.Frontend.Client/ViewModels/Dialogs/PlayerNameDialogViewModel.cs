using System;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace KC.Frontend.Client.ViewModels.Dialogs;

public partial class PlayerNameDialogViewModel : ReactiveObject, IValidatableViewModel
{
    [Reactive]
    private string _playerName;
    
    [Reactive]
    private bool _isRename;

    public IValidationContext ValidationContext { get; } = new ValidationContext();

    public PlayerNameDialogViewModel(string? originalName)
    {
        PlayerName = originalName ?? string.Empty;
        IsRename = !string.IsNullOrEmpty(originalName);
        this.ValidationRule(vm => vm.PlayerName, name => !string.IsNullOrEmpty(name), "Name cannot be empty");
    }

    private IObservable<bool> SaveCanExecute => this.IsValid();
    
    [ReactiveCommand(CanExecute = nameof(SaveCanExecute))]
    private void Save()
    { }
}