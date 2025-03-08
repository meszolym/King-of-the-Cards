using KC.Backend.Models.GameManagement;

namespace KC.Backend.Models.DTOs;

public class SessionDTO
{
    public Guid Id { get; }
    public TableDTO Table { get; }
    public TurnInfo CurrentTurnInfo { get; }
    public bool CanPlaceBets { get; }
}