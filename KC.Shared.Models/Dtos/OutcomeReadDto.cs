using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public record OutcomeReadDto(Guid sessionId, int boxIdx, int handIdx, Outcome outcome);