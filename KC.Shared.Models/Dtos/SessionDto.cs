using KC.Shared.Models.GameManagement;

namespace KC.Shared.Models.Dtos;

public record SessionDto(Guid Id, TableDto Table, TurnInfo CurrentTurnInfo, bool CanPlaceBets);