using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record struct SessionJoinLeaveDto(Guid SessionId, MacAddress Address);