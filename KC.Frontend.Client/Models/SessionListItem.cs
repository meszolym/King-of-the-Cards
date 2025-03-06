using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Frontend.Client.Models
{
    public class SessionListItem
    {
        public Guid Id { get; set; }
        public int MaxOccupancy { get; set; } = 5;
        public int CurrentOccupancy { get; set; } = 0;
        public string Occupancy => $"{CurrentOccupancy}/{MaxOccupancy}";

    }
}
