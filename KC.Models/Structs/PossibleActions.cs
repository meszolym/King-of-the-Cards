﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Structs;
public record struct PossibleActions(bool CanHit, bool CanDouble, bool CanSplit);