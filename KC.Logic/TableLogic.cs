using KC.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KC.Logic
{
    public class TableLogic
    {
        public static Table CreateTable(int numberOfBoxes)
        {
            Table table = new Table();
            table.Boxes = Enumerable.Range(0, numberOfBoxes)
                .Select(i =>
                    new BettingBox(table, i, Option<Player>.None, []))
                .ToImmutableList();
            return table;
        }

    }
}
