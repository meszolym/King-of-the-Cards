using KC.Shared.Models.Dtos;

namespace KC.Backend.API.Services.Interfaces;

public interface IBetOrchestrator
{
    void UpdateBet(BoxBetUpdateDto dto);
}