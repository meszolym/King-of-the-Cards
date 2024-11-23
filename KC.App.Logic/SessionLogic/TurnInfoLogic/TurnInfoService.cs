using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.App.Models.Classes;

namespace KC.App.Logic.SessionLogic.TurnInfoLogic
{
    public static class TurnInfoService
    {
        public static TurnInfo CreateEmptyTurnInfo() => new(false, 0, 0);
    }
}
