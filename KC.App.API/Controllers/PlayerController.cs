using KC.App.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KC.App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController(IPlayerLogic logic) : Controller
    {
        [HttpGet]
        public IActionResult GetPlayer(string Id) => logic.Get(Id).Match<IActionResult>(
            Some: player => Ok(player),
            None: NotFound
        );



    }
}
