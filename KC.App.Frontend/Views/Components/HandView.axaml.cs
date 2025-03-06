using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KC.App.Frontend.ViewModels;

namespace KC.App.Frontend.Views.Components;

public partial class HandView : ReactiveUserControl<HandViewModel>
{
    public HandView()
    {
        InitializeComponent();
    }
}