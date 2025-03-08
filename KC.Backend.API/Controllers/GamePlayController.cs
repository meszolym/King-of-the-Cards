using KC.Backend.Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GamePlayController(IGamePlayLogic gamePlayLogic) : ControllerBase
    {
    }
}
