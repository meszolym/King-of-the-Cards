using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;

namespace KC.Backend.API.Services.Interfaces;

public interface IBetOrchestrator
{
    Task UpdateBet(MacAddress address, BoxBetUpdateDto dto);
}