using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.App.Models.Classes;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic
{
    public static class HandService
    {
        public static Hand CreateEmptyHand() => new Hand([],0,true);
    }
}
