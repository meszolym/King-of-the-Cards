using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public record HandDto(IEnumerable<Card> Cards, double Bet);