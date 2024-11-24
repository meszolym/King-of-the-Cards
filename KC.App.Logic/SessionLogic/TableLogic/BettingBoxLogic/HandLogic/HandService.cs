using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.App.Models.Classes.Hand;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic
{
    public static class HandService
    {
        public static PlayerHand CreateEmptyPlayerHand() => new([],0,true);
    }
}
