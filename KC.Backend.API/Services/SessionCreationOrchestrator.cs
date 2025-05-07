using KC.Backend.API.Extensions;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services.Interfaces;
using KC.Shared.Models.Misc;
using NuGet.Protocol.Plugins;

namespace KC.Backend.API.Services;

public class SessionCreationOrchestrator(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IGamePlayLogic gamePlayLogic, ISessionTerminatorService sessionTerminatorService, IClientCommunicator hub) : ISessionCreationOrchestrator
{
    
    private const uint DefaultBoxes = 5;
    private const uint DefaultDecks = 8;
    private const int DefaultShuffleCardPlacement = -40;
    private const uint DefaultShuffleCardRange = 10;
    private const uint DefaultBettingTimeSpanSecs = 15;
    //private const uint DefaultSessionDestructionTimeSpanSecs = 10; 
    private const uint DefaultSessionDestructionTimeSpanSecs = 5*60; // 5 minutes
    
    public async Task CreateSession()
    {
        var sess = sessionLogic.CreateSession(DefaultBoxes, DefaultDecks, DefaultShuffleCardPlacement, DefaultShuffleCardRange, TimeSpan.FromSeconds(DefaultBettingTimeSpanSecs), TimeSpan.FromSeconds(DefaultSessionDestructionTimeSpanSecs));
        sess.DestructionTimer.Elapsed += async (sender, args) => await OnDestructionTimerElapsed(sess.Id);
        await hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionCreated, sess.ToDto(g => playerLogic.Get(g).Name));
        sess.BettingTimer.Tick += async (sender, args) => await OnBettingTimerTicked(sess.Id, sess.BettingTimer.RemainingSeconds);
        sess.BettingTimer.Elapsed += async (sender, args) => await OnBettingTimerElapsed(sess.Id);
        sess.TurnInfoChanged += async (sender, args) => await OnTurnInfoChanged(sess.Id);
    }

    private async Task OnTurnInfoChanged(Guid sessId)
    {
        var session = sessionLogic.Get(sessId);
        await hub.SendMessageToGroupAsync(sessId, SignalRMethods.TurnChanged, session.CurrentTurnInfo);
        if (!session.CurrentTurnInfo.PlayersTurn)
        {
            gamePlayLogic.DealerPlayHand(sessId);
        }
        
        //TODO: Cleanup here? Payout + clear hands + etc
    }

    private const int DelayBetweenCards = 2000;
    private async Task OnBettingTimerElapsed(Guid sessId)
    {
        await hub.SendMessageToGroupAsync(sessId, SignalRMethods.BettingTimerElapsed, sessId);

        gamePlayLogic.ShuffleIfNeeded(sessId);
        
        gamePlayLogic.DealHalfOfStartingCards(sessId);
        var session = sessionLogic.Get(sessId);
        await hub.SendMessageToGroupAsync(sessId, SignalRMethods.HandsUpdated, session.ToDto(g => playerLogic.Get(g).Name));
        await Task.Delay(DelayBetweenCards).ContinueWith(_ => gamePlayLogic.DealHalfOfStartingCards(sessId));
        await hub.SendMessageToGroupAsync(sessId, SignalRMethods.HandsUpdated, session.ToDto(g => playerLogic.Get(g).Name));
        gamePlayLogic.TransferTurn(sessId);
        await hub.SendMessageToGroupAsync(sessId, SignalRMethods.TurnChanged, session.CurrentTurnInfo);
    }
    private async Task OnBettingTimerTicked(Guid sessionId, int remainingSeconds) => await hub.SendMessageToGroupAsync(sessionId, SignalRMethods.BettingTimerTicked, (sessionId, remainingSeconds));
    
    private async Task OnDestructionTimerElapsed(Guid id)
    {
        var refundedPlayers = sessionTerminatorService.RefundAndRemoveSession(id).Select(playerLogic.Get).ToList();

        var connected = hub.ConnectionsAndGroups.Where(x => x.Value == id.ToString()).ToList();
        foreach (var conn in connected)
        {
            await hub.MoveToGroupAsync(conn.Key, hub.BaseGroup);
        }
        await hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionDeleted, id);
        
        foreach (var p in refundedPlayers)
        {
            await hub.SendMessageAsync(p.ConnectionId, SignalRMethods.PlayerBalanceUpdated, p.ToDto());
        }
    }
}