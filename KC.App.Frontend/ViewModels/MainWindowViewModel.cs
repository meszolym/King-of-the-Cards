using KC.App.Frontend.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;

namespace KC.App.Frontend.ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject, IScreen
    {
        public MainWindowViewModel()
        {
            Router.Navigate.Execute(new MenuViewModel(this));
        }

        public RoutingState Router { get; } = new();
    }
}
