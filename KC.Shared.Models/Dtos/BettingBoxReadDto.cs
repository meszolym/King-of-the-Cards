using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Shared.Models.Dtos;
public record BettingBoxReadDto(int BoxIdx, Guid OwnerId, string OwnerName, IEnumerable<HandReadDto> Hands);