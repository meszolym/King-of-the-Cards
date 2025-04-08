using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record struct BoxOwnerUpdateDto(Guid SessionId, int BoxIdx, MacAddress OwnerMac);