using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Logic.Services.Interfaces;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Services;

public class SessionTerminatorService(ISessionLogic sessionLogic, IPlayerLogic playerLogic) : ISessionTerminatorService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The list of players that received a refund</returns>
    public IEnumerable<MacAddress> RefundAndRemoveSession(Guid id)
    {
        var destroyedSession = sessionLogic.RemoveSession(id);
        List<MacAddress> refundedPlayers = [];
        
        //refund all players
        foreach (var box in destroyedSession.Table.BettingBoxes.Where(b => b.OwnerId != MacAddress.None))
        {
            foreach (var hand in box.Hands.Where(h => h.Bet > 0))
            {
                var p = playerLogic.Get(box.OwnerId);
                playerLogic.UpdateBalance(box.OwnerId, p.Balance+hand.Bet);
                refundedPlayers.Add(p.Mac);
            }
        }
        return refundedPlayers;
    }
}