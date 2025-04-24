using System;
using System.Linq;
using KC.Frontend.Client.Models;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;

namespace KC.Frontend.Client.Extensions;

public static class ConversionExtensions
{
    public static SessionListItem ToSessionListItem(this SessionReadDto s) =>
        new()
        {
            Id = s.Id, CurrentOccupancy = s.Table.BettingBoxes.Count(b => b.OwnerId != Guid.Empty),
            MaxOccupancy = s.Table.BettingBoxes.Count()
        };
}