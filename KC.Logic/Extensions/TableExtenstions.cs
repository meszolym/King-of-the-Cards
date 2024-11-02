using KC.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Logic.Extensions
{
    public static class TableExtenstions
    {
        //Setters that give back the item.
        public static Table SetBoxes(this Table table, ImmutableList<BettingBox> boxes)
        {
            table.Boxes = boxes;
            return table;
        }

        public static Table SetShoe(this Table table, Option<ShuffledShoe> shoe)
        {
            table.Shoe = shoe;
            return table;
        }
    }
}
