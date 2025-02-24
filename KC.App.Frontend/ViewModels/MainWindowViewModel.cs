using KC.App.Frontend.Models;
using System.Collections.Generic;

namespace KC.App.Frontend.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        List<SessionListItem> Sessions = new List<SessionListItem>();

        public MainWindowViewModel()
        {
            Sessions.Add(new()
            {
                Id = System.Guid.NewGuid()
            });
        }
    }
}
