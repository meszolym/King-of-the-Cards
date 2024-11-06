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
        public static Table CreateTable(ShoeProperties shoeProperties)
            => new(Id: Guid.NewGuid(),
                ShoeProperties: shoeProperties,
                CurrentHandInTurn: -1 // -1 means no hand is in turn, round has not started
            );

        public static IEnumerable<BettingBox> CreateBoxes(Table table, int numberOfBoxes)
            => Enumerable.Range(0, numberOfBoxes)
                .Select(i => new BettingBox(table.Id, i, Option<string>.None, 
                    [new Hand(table.Id, i, 0,true, [], 0)]));

    }
}
