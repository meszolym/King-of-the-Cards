using KC.Data;
using KC.Logic.SessionLogic.TableLogic;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Models.Classes;
using LanguageExt;
using System.Reactive;
using System.Reactive.Linq;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Unit = LanguageExt.Unit;

namespace KC.Logic.SessionLogic;

internal class SessionLogic(IDataStore<Session, Guid> dataStore)
{
    public Fin<Unit> CreateSession(uint numberOfBoxes, uint numberOfDecks, TimeSpan timeAfterFirstBet) =>
        dataStore.Add(SessionService.CreateEmptySession(numberOfBoxes, numberOfDecks, timeAfterFirstBet));

    public IEnumerable<Session> GetAllSessions() => dataStore.GetAll();

    public Fin<BettingBox> ClaimBox(Guid sessionId, int boxIdx, Player player) =>
        dataStore.Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx)).Bind(b =>b.Claim(player));

    public Fin<BettingBox> UnclaimBox(Guid sessionId, int boxIdx, Player player) =>
        dataStore.Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx)).Bind(b => b.Unclaim(player));

    public Fin<BettingBox> PlaceBet(Guid sessionId, int boxIdx, Player player, double amount) => dataStore
        .Get(sessionId).Bind(s => s.CanPlaceBets
            ? HandleBetting(s, boxIdx, player, amount)
            : FinFail<BettingBox>(Error.New("Can not place bets at this time.")));

    private Fin<BettingBox> HandleBetting(Session session, int boxIdx, Player player, double amount) =>
        PutBetOnBox(session.Id, boxIdx, player, amount).Map(b =>
        {
            if (!session.Table.Boxes.Any(box => box.Hands[0].Bet > 0))
            {
                session.BetPlacementTimer.Stop();
            }
            else if (amount > 0 && !session.BetPlacementTimer.Enabled)
            {
                session.BetPlacementTimer.Start();
            }
            return b;
        });

    public Fin<BettingBox> PutBetOnBox(Guid sessionId, int boxIdx, Player player, double amount) =>
        dataStore.Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx)
            .Bind(b => b.PlaceBet(player, amount)));

    public Fin<(int boxIdx, int handIdx)> GetCurrentTurn(Guid sessionId) =>
        dataStore.Get(sessionId).Map(s => (s.CurrentBoxIdx, s.CurrentHandIdx));

    //getmoves

    //makemove -> if no more moves, transfer turn to next player

    //transferturn -> if everyone has played, fire and forget endround

    //endround (if everyone has played)
}