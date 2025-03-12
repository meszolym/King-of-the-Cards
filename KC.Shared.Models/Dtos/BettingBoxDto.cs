using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;
public record BettingBoxDto(MacAddress OwnerId, IEnumerable<HandDto> Hands);