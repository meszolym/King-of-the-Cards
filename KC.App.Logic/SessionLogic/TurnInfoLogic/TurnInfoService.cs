using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.App.Models.Classes;

namespace KC.App.Logic.SessionLogic.TurnInfoLogic
{
    internal static class TurnInfoService
    {
        internal static TurnInfo CreateEmptyTurnInfo() => new(false, 0, 0);
    }
}
