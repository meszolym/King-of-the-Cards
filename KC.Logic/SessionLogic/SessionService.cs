using System.Collections.Immutable;
using System.Windows.Markup;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.Models.Classes;
using KC.Models.Records;
using Timer = System.Timers.Timer;
using LanguageExt;

namespace KC.Logic.SessionLogic;

public static class SessionService
{
    public static Session CreateEmptySession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet) =>
        new Session(Guid.NewGuid(),
            new Table(
                Enumerable.Range(0, (int)numberOfBoxes).Select(i => BettingBoxService.CreateEmptyBettingBox())
                    .ToImmutableList(),
                ShoeService.CreateUnshuffledShoe(numberOfBoxes)), timerAfterFirstBet).AddCanBetChangeOnTimerElapsed();

    //although this is an extension, it is put here because it is only used in the context of creating a session
    private static Session AddCanBetChangeOnTimerElapsed(this Session session)
    {
        session.BetPlacementTimer.Elapsed += (sender, args) =>
        {
            session.CanPlaceBets = false;
            session.BetPlacementTimer.Stop();
        };
        return session;
    }
}
