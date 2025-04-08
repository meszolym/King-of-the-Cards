using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record struct PlayerRegisterDto(string Name, MacAddress Mac);