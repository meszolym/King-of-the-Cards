using System.Collections.Immutable;
using KC.App.Logic.SessionLogic.BettingBoxLogic;
using KC.App.Logic.SessionLogic.ShoeLogic;
using KC.App.Logic.SessionLogic.TurnInfoLogic;
using KC.App.Models.Classes;
using KC.App.Models.Classes.Hand;
using Timer = System.Timers.Timer;

namespace KC.App.Logic.SessionLogic;

internal static class SessionService
{
    internal static Session CreateEmptySession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet) =>
        new Session(Guid.NewGuid(),
            Enumerable.Range(0, (int)numberOfBoxes).Select(BettingBoxService.CreateEmptyBettingBox).ToImmutableList(), 
            ShoeService.CreateUnshuffledShoe(numberOfBoxes), 
            new DealerHand([]),
            timerAfterFirstBet,
            TurnInfoService.CreateEmptyTurnInfo());

}
