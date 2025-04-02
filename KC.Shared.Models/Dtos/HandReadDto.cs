using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public record HandReadDto(IEnumerable<Card> Cards, double Bet);