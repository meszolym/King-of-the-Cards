using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record PossibleActions(bool CanHit, bool CanDouble, bool CanSplit)
    {
        public void Deconstruct(out bool CanHit, out bool CanDouble, out bool CanSplit)
        {
            CanHit = this.CanHit;
            CanDouble = this.CanDouble;
            CanSplit = this.CanSplit;
        }
    }
}
