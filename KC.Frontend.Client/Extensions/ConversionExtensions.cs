using System;
using System.Linq;
using KC.Frontend.Client.ViewModels.Components;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;

namespace KC.Frontend.Client.Extensions;

public static class ConversionExtensions
{
    public static SessionListItemViewModel ToSessionListItem(this SessionReadDto s) =>
        new()
        {
            Id = s.Id, CurrentOccupancy = s.Table.BettingBoxes.Count(b => b.OwnerId != Guid.Empty),
            MaxOccupancy = s.Table.BettingBoxes.Count(),
            TooltipText = s.Table.BettingBoxes.Count(b => b.OwnerId != Guid.Empty) == 0 
                ? "Players:" + Environment.NewLine + "None" 
                : "Players:" + Environment.NewLine + string.Join(Environment.NewLine, s.Table.BettingBoxes.Select(b => b.OwnerName).Distinct())
        };
}