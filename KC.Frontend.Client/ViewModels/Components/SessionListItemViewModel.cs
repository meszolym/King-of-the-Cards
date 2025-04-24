using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels.Components
{
    public partial class SessionListItemViewModel : ReactiveObject
    {
        public Guid Id { get; set; }
        public int MaxOccupancy { get; init; } = 5; //5 are drawn on the screen, but the server can have more, we cannot handle that
        
        [Reactive]
        private int _currentOccupancy = 0;
        [Reactive] 
        private string _occupancy = "";

        public SessionListItemViewModel()
        {
            this.WhenAnyValue(vm => vm.CurrentOccupancy).ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ =>
            {
                Occupancy = $"{CurrentOccupancy}/{MaxOccupancy}";
            });
        }
    }
}
