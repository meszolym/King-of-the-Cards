using KC.Shared.Models.GameManagement;

namespace KC.Shared.Models.Dtos;

public record SessionReadDto(Guid Id, TableReadDto Table, TurnInfo CurrentTurnInfo, bool CanPlaceBets);