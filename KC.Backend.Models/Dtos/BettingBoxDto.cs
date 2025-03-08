namespace KC.Backend.Models.Dtos;

public class BettingBoxDto
{
    public Guid OwnerId { get; init; }
    public IEnumerable<HandDto> Hands { get; init; }
}