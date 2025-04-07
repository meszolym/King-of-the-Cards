using System.Net.NetworkInformation;

namespace KC.Shared.Models.Dtos;

public record PlayerReadDto(Guid Id, string Name, double Balance);