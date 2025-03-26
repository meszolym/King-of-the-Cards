using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;

public record PlayerDto(string Name, double Balance);

public record PlayerRegisterDto(string Name, MacAddress MacAddress);