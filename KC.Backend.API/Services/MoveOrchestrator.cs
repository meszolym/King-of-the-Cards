using KC.Backend.API.Extensions;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.API.Services;

public class MoveOrchestrator(IPlayerLogic playerLogic, IGamePlayLogic gamePlayLogic, ISessionLogic sessionLogic, IClientCommunicator hub) : IMoveOrchestrator
{
    public async Task MakeMove(MacAddress macAddress, MakeMoveDto dto)
    {
        var player = playerLogic.Get(macAddress);
        var session = sessionLogic.Get(dto.sessionId);
        var box = session.Table.BettingBoxes[dto.boxIdx];
        var hand = box.Hands[dto.handIdx];
        
        if (!gamePlayLogic.GetPossibleActionsOnHand(hand).Contains(dto.move)) throw new InvalidOperationException("Invalid move.");
        
        if (dto.move is Move.Double or Move.Split)
        {
            if (player.Balance <= hand.Bet) throw new InvalidOperationException("Player balance does not cover the bet.");
            playerLogic.AddToBalance(macAddress,-hand.Bet);
            await hub.SendMessageAsync(player.ConnectionId, SignalRMethods.PlayerBalanceUpdated, player.ToDto());
        }
        gamePlayLogic.MakeMove(dto.sessionId, dto.boxIdx, macAddress, dto.move, dto.handIdx);
        await hub.SendMessageToGroupAsync(dto.sessionId, SignalRMethods.HandsUpdated, session.ToDto(g => playerLogic.Get(g).Name));

        if (dto.move is Move.Double or Move.Split)
        {
            switch (dto.move)
            {
                case Move.Double:
                    hand.Bet *= 2;
                    break;
                case Move.Split:
                {
                    var otherHand = box.Hands.Last();
                    otherHand.Bet = hand.Bet;
                    break;
                }
            }
            
            await hub.SendMessageToGroupAsync(dto.sessionId, SignalRMethods.BetUpdated, box.ToDto(g => playerLogic.Get(g).Name));
        }
        
        await TransferTurn(dto.sessionId);
    }

    public async Task TransferTurn(Guid sessionId)
    {
        var sess = sessionLogic.Get(sessionId);
        await hub.SendMessageToGroupAsync(sessionId, SignalRMethods.TurnChanged, sess.CurrentTurnInfo);
    }
}