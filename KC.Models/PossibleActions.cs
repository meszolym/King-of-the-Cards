﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record PossibleActions
    {
        public bool CanHit { get; }
        public bool CanDouble { get; }
        public bool CanSplit { get; }

        public PossibleActions(bool CanHit, bool CanDouble, bool CanSplit)
        {
            this.CanHit = CanHit;
            this.CanDouble = CanDouble;
            this.CanSplit = CanSplit;
        }

        public PossibleActions(){}

        public PossibleActions AddHit() => new(true, this.CanDouble, this.CanSplit);
        public PossibleActions AddDouble() => new(this.CanHit, true, this.CanSplit);
        public PossibleActions AddSplit() => new(this.CanHit, this.CanDouble, true);
    } 
}
