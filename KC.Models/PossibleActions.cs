using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record PossibleActions(bool CanHit, bool CanDouble, bool CanSplit)
    {
        public bool CanHit { get; set; } = CanHit;
        public bool CanDouble { get; set; } = CanDouble;
        public bool CanSplit { get; set; } = CanSplit;
    }
}
