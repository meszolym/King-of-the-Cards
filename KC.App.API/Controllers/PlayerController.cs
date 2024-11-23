using KC.App.Logic.Interfaces;
using KC.App.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace KC.App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController(IPlayerLogic logic) : Controller
    {
        [HttpGet]
        public Player GetPlayer(string Id) => logic.Get(Id);

        [HttpPost]
        public void AddPlayer(Player player) => logic.Add(player);

        [HttpDelete]
        public void RemovePlayer(string Id) => logic.Remove(Id);

    }
}
