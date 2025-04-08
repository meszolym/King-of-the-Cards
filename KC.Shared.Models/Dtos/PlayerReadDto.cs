using System.Net.NetworkInformation;

namespace KC.Shared.Models.Dtos;

public record struct PlayerReadDto(Guid Id, string Name, double Balance);