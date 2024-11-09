using System.Collections.Immutable;
using System.Windows.Markup;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.Models.Classes;
using KC.Models.Records;

namespace KC.Logic.SessionLogic;

public class SessionService
{
    public static Session CreateEmptySession(uint numberOfBoxes, uint numberOfDecks) => new Session(Guid.NewGuid(),
        new Table(Enumerable.Range(0,(int)numberOfBoxes).Select(i => BettingBoxService.CreateEmptyBettingBox()).ToImmutableList(),
            ShoeService.CreateUnshuffledShoe(numberOfBoxes)));
}
