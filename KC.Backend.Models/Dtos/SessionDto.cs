using KC.Backend.Models.GameManagement;

namespace KC.Backend.Models.Dtos;

public class SessionDto
{
    public Guid Id { get; init; }
    public TableDto Table { get; init; }
    public TurnInfo CurrentTurnInfo { get; init; }
    public bool CanPlaceBets { get; init; }
}