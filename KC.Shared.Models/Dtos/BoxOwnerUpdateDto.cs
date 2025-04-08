using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record BoxOwnerUpdateDto(Guid SessionId, int BoxIdx, MacAddress OwnerMac);