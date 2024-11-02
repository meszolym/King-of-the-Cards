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
            => new Table(Id: Guid.NewGuid(),
                ShoeProperties: shoeProperties,
                Boxes: Enumerable.Range(0, numberOfBoxes)
                    .Select(i => new BettingBox(Option<Player>.None,
                        [new Hand(true, [], 0)]))
                    .ToImmutableList(),
                CurrentHandInTurn: -1 // -1 means no hand is in turn, round has not started
                );

    }
}
