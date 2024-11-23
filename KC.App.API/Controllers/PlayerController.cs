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



    }
}
