using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Logic.ShoeLogic;
using KC.Models.Classes;
using KC.Models.Records;

namespace KC.Logic.TableLogic;

public static class TableService
{
    public static Table CreateTable(int numberOfBoxes, int numberOfDecks) => new Table(
        Enumerable.Range(0, numberOfBoxes).Select(i => new BettingBox(i,new List<Hand>())).ToImmutableList(),
        ShoeService.CreateShoe(numberOfDecks));
}