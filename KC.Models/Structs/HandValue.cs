using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Structs;


//TODO: Create a builder for this struct
public record struct HandValue(int Value, bool IsBlackJack, bool IsPair, bool IsSoft);