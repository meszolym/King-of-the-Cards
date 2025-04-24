using KC.Shared.Models.Dtos;

namespace KC.Backend.API.Services.Interfaces;

public interface IBetOrchestrator
{
    Task UpdateBet(BoxBetUpdateDto dto);
}