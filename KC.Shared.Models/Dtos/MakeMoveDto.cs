using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public record MakeMoveDto(Guid sessionId, int boxIdx, int handIdx, Move move);