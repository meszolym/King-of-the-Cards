using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;
public record BettingBoxReadDto(MacAddress OwnerId, IEnumerable<HandReadDto> Hands);