using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.App.Models.Classes.Hand;

namespace KC.App.Logic.SessionLogic.HandLogic
{
    internal static class HandService
    {
        internal static PlayerHand CreateEmptyPlayerHand() => new([], 0, true);
    }
}
