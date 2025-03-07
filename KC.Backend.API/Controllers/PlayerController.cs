using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController(IPlayerLogic logic, ISessionLogic sessionLogic) : Controller
{
    [HttpGet("{Id}")]
    public Player GetPlayer(string Id) => logic.Get(Id);

    [HttpPost]
    public void AddPlayer([FromBody] Player player) => logic.Add(player);

    [HttpDelete("{Id}")]
    public void RemovePlayer(string Id)
    {
        bool removable = sessionLogic.GetAllSessions()
            .Any(s => s.Boxes.Any(b => b.Owner.Equals(logic.Get(Id))));

        if (!removable) throw new InvalidOperationException("Player is still in a game.");

        logic.Remove(Id);
    }

    [HttpPut]
    public void UpdatePlayer([FromBody] Player player) => logic.Update(player);

}