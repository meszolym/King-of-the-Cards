using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record PossibleActions(bool CanHit, bool CanDouble, bool CanSplit)
    {
        public bool CanHit = CanHit;
        public bool CanDouble = CanDouble;
        public bool CanSplit = CanSplit;
    }
}
