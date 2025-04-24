using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;

namespace KC.Backend.API.Services;

public class BetOrchestrator(IBettingBoxLogic bettingBoxLogic, IPlayerLogic playerLogic, ISessionLogic sessionLogic, IClientCommunicator hub) : IBetOrchestrator
{
    public void UpdateBet(BoxBetUpdateDto dto)
    {
        var player = playerLogic.Get(dto.OwnerMac);

        var alreadyPlaced = bettingBoxLogic.GetBetOnBox(dto.SessionId, dto.BoxIdx, dto.HandIdx);
        
        if (dto.Amount - alreadyPlaced > player.Balance)
            throw new InvalidOperationException("Player balance does not cover the bet.");
        
        bettingBoxLogic.UpdateBetOnBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac, dto.Amount, dto.HandIdx);
        playerLogic.UpdateBalance(dto.OwnerMac, player.Balance - (dto.Amount - alreadyPlaced));
        
        hub.SendMessageAsync(player.ConnectionId, "PlayerBalanceUpdated", player.ToDto());
        hub.SendMessageAsync(dto.SessionId.ToString(), "BetUpdated", bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto());
        
        sessionLogic.UpdateBettingTimer(dto.SessionId);
    }
}