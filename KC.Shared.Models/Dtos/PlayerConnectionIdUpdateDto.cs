using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record PlayerConnectionIdUpdateDto(MacAddress Id, string ConnectionId);