using System.Diagnostics;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class GamePlayController(IGamePlayLogic gamePlayLogic, ISessionLogic sessionLogic) : Controller
{
    [HttpGet("get-moves/{sessionId:guid}/{boxIdx:int}/{handIdx:int}")]
    public IEnumerable<Move> GetPossibleMovesOnHand(Guid sessionId, int boxIdx, int handIdx) => gamePlayLogic.GetPossibleActionsOnHand(sessionLogic.Get(sessionId).Table.BettingBoxes[boxIdx].Hands[handIdx]);
    
}