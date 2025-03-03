using KC.App.Frontend.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.App.Frontend
{
    class AppViewLocator : IViewLocator
    {
        public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
        {
            SessionViewModel ctx => new Views.SessionView() { ViewModel = ctx },
            MenuViewModel ctx => new Views.MenuView() { ViewModel = ctx },
            _ => null
        };
    }
}
