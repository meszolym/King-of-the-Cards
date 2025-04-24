using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;

namespace KC.Backend.API.Services;

public class BetOrchestrator(IBettingBoxLogic bettingBoxLogic, IPlayerLogic playerLogic, ISessionLogic sessionLogic, IClientCommunicator hub) : IBetOrchestrator
{
    public async Task UpdateBet(BoxBetUpdateDto dto)
    {
        var player = playerLogic.Get(dto.OwnerMac);

        var alreadyPlaced = bettingBoxLogic.GetBetOnBox(dto.SessionId, dto.BoxIdx, dto.HandIdx);
        
        if (dto.Amount - alreadyPlaced > player.Balance)
            throw new InvalidOperationException("Player balance does not cover the bet.");
        
        bettingBoxLogic.UpdateBetOnBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac, dto.Amount, dto.HandIdx);
        playerLogic.UpdateBalance(dto.OwnerMac, player.Balance - (dto.Amount - alreadyPlaced));
        
        await hub.SendMessageAsync(player.ConnectionId, SignalRMethods.PlayerBalanceUpdated, player.ToDto());
        var dtoToSend = bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto(g => playerLogic.Get(g).Name);
        await hub.SendMessageToGroupAsync(dto.SessionId.ToString(), SignalRMethods.BetUpdated, dtoToSend);
        
        sessionLogic.UpdateBettingTimer(dto.SessionId);
    }
}