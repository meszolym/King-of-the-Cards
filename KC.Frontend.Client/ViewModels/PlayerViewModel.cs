using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels;

public partial class PlayerViewModel : ReactiveObject
{
    [Reactive]
    private string _playerName;
        
    [Reactive]
    private double _playerBalance;
}