using System.Collections.Immutable;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.Models.Classes;
using KC.Models.Records;
using Timer = System.Timers.Timer;

namespace KC.Logic.SessionLogic;

public static class SessionService
{
    public static Session CreateEmptySession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet) =>
        new Session(Guid.NewGuid(),
            new Table(
                Enumerable.Range(0, (int)numberOfBoxes).Select(i => BettingBoxService.CreateEmptyBettingBox())
                    .ToImmutableList(),
                ShoeService.CreateUnshuffledShoe(numberOfBoxes)), timerAfterFirstBet);

}
