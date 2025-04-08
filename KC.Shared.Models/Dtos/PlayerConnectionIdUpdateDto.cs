using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record struct PlayerConnectionIdUpdateDto(MacAddress Id, string ConnectionId);