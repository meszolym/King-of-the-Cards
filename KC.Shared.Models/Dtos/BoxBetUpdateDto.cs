using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record struct BoxBetUpdateDto(Guid SessionId, int BoxIdx, double Amount);