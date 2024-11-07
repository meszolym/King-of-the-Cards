using System.Collections.Immutable;
using System.Windows.Markup;
using KC.Logic.ShoeLogic;
using KC.Logic.TableLogic;
using KC.Models.Classes;
using KC.Models.Records;

namespace KC.Logic.SessionLogic;

public class SessionService
{
    // unnecessary
    // public static Session CreateSession(Table table) => new Session(Guid.NewGuid(), table);

    public static Session CreateEmptySession(int numberOfBoxes, int numberOfDecks) => new Session(Guid.NewGuid(),
        new Table(Enumerable.Range(0,numberOfBoxes).Select(i => new BettingBox()).ToImmutableList(),
            ShoeService.CreateShoe(numberOfBoxes)));
}
