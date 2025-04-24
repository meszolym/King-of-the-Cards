using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services.Interfaces;

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
        await hub.SendMessageToGroupAsync(hub.BaseGroup, "SessionCreated", sess.ToDto(g => playerLogic.Get(g).Name));
        sess.BettingTimer.Tick += async (sender, args) => await OnBettingTimerTicked(sess.Id);
        sess.BettingTimer.Elapsed += async (sender, args) => await OnBettingTimerElapsed(sess.Id);
    }

    private async Task OnBettingTimerElapsed(Guid sessId)
    {
        await hub.SendMessageToGroupAsync(sessId.ToString(), "BettingTimerElapsed", sessId);
        gamePlayLogic.DealStartingCards(sessId);
        var session = sessionLogic.Get(sessId);
        await hub.SendMessageToGroupAsync(sessId.ToString(), "HandsUpdated", session.ToDto(g => playerLogic.Get(g).Name));
    }
    private async Task OnBettingTimerTicked(Guid sessionId) => await hub.SendMessageToGroupAsync(sessionId.ToString(), "BettingTimerTicked", sessionId);
    
    private async Task OnDestructionTimerElapsed(Guid id)
    {
        var refundedPlayers = sessionTerminatorService.RefundAndRemoveSession(id).Select(playerLogic.Get).ToList();

        var connected = hub.ConnectionsAndGroups.Where(x => x.Value == id.ToString()).ToList();
        foreach (var conn in connected)
        {
            await hub.MoveToGroupAsync(conn.Key, hub.BaseGroup);
        }
        await hub.SendMessageToGroupAsync(hub.BaseGroup, "SessionDeleted", id);
        
        foreach (var p in refundedPlayers)
        {
            await hub.SendMessageAsync(p.ConnectionId, "PlayerBalanceUpdated", p.ToDto());
        }
    }
}