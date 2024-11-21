using KC.App.Models.Classes;

namespace KC.App.Logic.SessionLogic;
public static class SessionExtensions
{
    public static Session AddCanBetChangeOnTimerElapsed(this Session session)
    {
        session.BetPlacementTimer.Elapsed += (sender, args) =>
        {
            session.CanPlaceBets = false;
            session.BetPlacementTimer.Stop();
        };
        return session;
    }
}