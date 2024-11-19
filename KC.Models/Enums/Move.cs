using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Enums
{
    /// <summary>
    /// The possible moves in a game of blackjack. Stand is not included as it's always a possibility.
    /// </summary>
    public enum Move
    {
        Hit,
        Double,
        Split,
    }
}
