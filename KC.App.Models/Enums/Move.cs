using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.App.Backend.Models.Enums
{
    /// <summary>
    /// The possible moves in a game of blackjack.
    /// </summary>
    public enum Move
    {
        Stand,
        Hit,
        Double,
        Split,
    }
}
