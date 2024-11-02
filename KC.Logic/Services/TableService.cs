using KC.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KC.Logic.Services
{
    public static class TableService
    {
        public static Table CreateTable(int numberOfBoxes, ShoeProperties shoeProperties)
        {
            Table table = new Table(shoeProperties);
            table.Boxes = Enumerable.Range(0, numberOfBoxes)
                .Select(i =>
                    new BettingBox(table, i, Option<Player>.None, []))
                .ToImmutableList();
            table.Shoe = ShoeService.CreateShuffledShoe(table.ShoeProperties);
            return table;
        }
    }
}
