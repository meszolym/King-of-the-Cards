using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;

namespace KC.Backend.API.Services;

public interface IMoveOrchestrator
{
    Task MakeMove(MacAddress macAddress, MakeMoveDto dto);
}