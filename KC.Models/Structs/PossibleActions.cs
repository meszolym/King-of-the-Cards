using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Structs;

//TODO: Create a builder for this struct
public record struct PossibleActions(bool CanHit, bool CanDouble, bool CanSplit);