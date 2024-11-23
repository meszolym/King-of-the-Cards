﻿using System.Collections.Immutable;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.App.Models.Classes;
using KC.App.Models.Records;

namespace KC.App.Logic.SessionLogic.TableLogic
{
    public static class TableExtensions
    {
        public static IEnumerable<BettingBox> BoxesInPlay(this Table table) =>
            table.Boxes.Where(box => box.Hands[0].Bet > 0).OrderBy(b => b.Idx);

        public static void Reset(this Table table)
        {
            table.Boxes.ForEach(b => b.ClearHands());
        }
    }
}
