using System.Collections.Immutable;
using KC.Logic.BettingBoxLogic;
using KC.Logic.ShoeLogic;
using KC.Models.Records;

namespace KC.Logic.TableLogic;

public static class TableService
{
    // goes against dependency injection.
    // public static Table CreateTable(int numberOfBoxes, int numberOfDecks) => new Table(
    //     Enumerable.Range(0, numberOfBoxes).Select(i => BettingBoxService.CreateBox()).ToImmutableList(), //create bettingbox via bettingboxService
    //     ShoeService.CreateShoe(numberOfDecks));
}