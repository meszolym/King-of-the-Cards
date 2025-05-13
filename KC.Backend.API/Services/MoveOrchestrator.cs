using KC.Backend.API.Extensions;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.API.Services;

public class MoveOrchestrator(IPlayerLogic playerLogic, IGamePlayLogic gamePlayLogic, ISessionLogic sessionLogic, IClientCommunicator hub, BetUpdatedDelegate betUpdated) : IMoveOrchestrator
{
    //TODO: Remove bet if bust
    //TODO: Timer for move, if not made, skip turn (auto-stand)
    //TODO: Move to logic layer into a service
    public async Task MakeMove(MacAddress macAddress, MakeMoveDto dto)
    {
        var player = playerLogic.Get(macAddress);
        var session = sessionLogic.Get(dto.sessionId);
        var box = session.Table.BettingBoxes[dto.boxIdx];
        var hand = box.Hands[dto.handIdx];
        
        if (!gamePlayLogic.CanMakeMove(dto.sessionId, dto.boxIdx, dto.handIdx, dto.move)) throw new InvalidOperationException("Invalid move."); 
        
        if (dto.move is Move.Double or Move.Split) //TODO: Make CanCoverBet in PlayerLogic
        {
            if (player.Balance <= hand.Bet) throw new InvalidOperationException("Player balance does not cover the bet.");
            playerLogic.AddToBalance(macAddress,-hand.Bet);
            await hub.SendMessageAsync(player.ConnectionId, SignalRMethods.PlayerBalanceUpdated, player.ToDto());
        }
        await gamePlayLogic.MakeMove(dto.sessionId, dto.boxIdx, macAddress, dto.move, dto.handIdx);
        switch (dto.move)
        {
            case Move.Stand or Move.Hit:
                return;
            case Move.Double:
                hand.Bet *= 2;
                break;
            case Move.Split:
                var otherHand = box.Hands.Last();
                otherHand.Bet = hand.Bet;
                break;
        }
        await betUpdated(dto.sessionId, dto.boxIdx);
    }
}