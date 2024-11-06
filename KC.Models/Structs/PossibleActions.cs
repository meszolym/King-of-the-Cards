using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Structs;

//TODO: Create a builder for this struct
public readonly struct PossibleActions(bool canHit, bool canDouble, bool canSplit)
{
    public bool CanHit { get; } = canHit;
    public bool CanDouble { get; } = canDouble;
    public bool CanSplit { get; } = canSplit;
}