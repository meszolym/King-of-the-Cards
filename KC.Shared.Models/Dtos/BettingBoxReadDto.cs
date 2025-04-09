using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;
public record BettingBoxReadDto(int BoxIdx, MacAddress OwnerId, IEnumerable<HandReadDto> Hands);