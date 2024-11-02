using KC.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace KC.Logic.Extensions
{
    public static class BettingBoxExtensions
    {
        //Setters that give back the item.
        public static BettingBox SetOwner(this BettingBox box, Option<Player> owner)
        {
            box.Owner = owner;
            return box;
        }

        public static Fin<BettingBox> SetBet(this BettingBox box, int amount)
        {
            if (amount >= 0)
            {
                box.Hands[0].Bet = amount;
                return box;
            }

            return Error.New("Bet cannot be negative.");
        }
    }
}
