using KC.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KC.Logic.Extensions;

namespace KC.Logic.Services
{
    public static class TableService
    {
        public static Table CreateTable(int numberOfBoxes, ShoeProperties shoeProperties) 
            => new Table(shoeProperties).SetBoxes(Enumerable.Range(0, numberOfBoxes)
                .Select(i => new BettingBox(Option<Player>.None, 
                [new Hand(true, [],0)]))
                .ToImmutableList()).SetShoe(ShoeService.CreateShuffledShoe(shoeProperties));
    }
}
