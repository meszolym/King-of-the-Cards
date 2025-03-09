using System.Net.NetworkInformation;

namespace KC.Shared.Models.Dtos;

public class BettingBoxDto
{
    public PhysicalAddress OwnerId { get; init; }
    public IEnumerable<HandDto> Hands { get; init; }
}