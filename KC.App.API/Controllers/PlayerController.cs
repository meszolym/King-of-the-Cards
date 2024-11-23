using KC.App.Logic.Interfaces;
using KC.App.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace KC.App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController(IPlayerLogic logic, ISessionLogic sessionLogic) : Controller
    {
        [HttpGet]
        public Player GetPlayer(string Id) => logic.Get(Id);

        [HttpPost]
        public void AddPlayer(Player player) => logic.Add(player);

        [HttpDelete]
        public void RemovePlayer(string Id)
        {
            bool removable = sessionLogic.GetAllSessions()
                .Any(s => s.Table.Boxes.Any(b => b.Owner.Equals(logic.Get(Id))));

            if (!removable) throw new InvalidOperationException("Player is still in a game.");

            logic.Remove(Id);
        }
    }
}
