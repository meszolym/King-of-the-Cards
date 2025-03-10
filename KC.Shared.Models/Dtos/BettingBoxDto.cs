using System.Net.NetworkInformation;

namespace KC.Shared.Models.Dtos;
public record BettingBoxDto(PhysicalAddress OwnerId, IEnumerable<HandDto> Hands);