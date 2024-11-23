using System.Collections.Immutable;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.App.Logic.SessionLogic.TurnInfoLogic;
using KC.App.Models.Classes;
using KC.App.Models.Records;
using Timer = System.Timers.Timer;

namespace KC.App.Logic.SessionLogic;

public static class SessionService
{
    public static Session CreateEmptySession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet) =>
        new Session(Guid.NewGuid(),
            new Table(
                Enumerable.Range(0, (int)numberOfBoxes).Select(BettingBoxService.CreateEmptyBettingBox).ToImmutableList(),
                ShoeService.CreateUnshuffledShoe(numberOfBoxes),
                new Hand([], 0, false)),
            timerAfterFirstBet,
            TurnInfoService.CreateEmptyTurnInfo());

}
