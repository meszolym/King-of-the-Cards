using System.Diagnostics;
using KC.Backend.API.Services;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class GamePlayController(IGamePlayLogic gamePlayLogic, ISessionLogic sessionLogic, IMoveOrchestrator moveOrchestrator) : Controller
{
    [HttpGet("get-moves/{sessionId:guid}/{boxIdx:int}/{handIdx:int}")]
    public IEnumerable<Move> GetPossibleMovesOnHand(Guid sessionId, int boxIdx, int handIdx) => gamePlayLogic.GetPossibleActionsOnHand(sessionLogic.Get(sessionId).Table.BettingBoxes[boxIdx].Hands[handIdx]);

    [HttpPost]
    [Route("make-move")]
    public void MakeMove([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, [FromBody] MakeMoveDto dto)
    {
        moveOrchestrator.MakeMove(MacAddress.Parse(macAddress), dto);
        var nextPossibleMoves = gamePlayLogic.GetPossibleActionsOnHand(dto.sessionId, dto.boxIdx, dto.handIdx).ToArray();
        if (!nextPossibleMoves.Any() || nextPossibleMoves.All(m => m == Move.Stand))
        {
            gamePlayLogic.TransferTurn(dto.sessionId);
            //Hub is called automatically, subscribed to when TurnInfo changes in session (see SessionCreationOrchestrator) 
        }
    }
}