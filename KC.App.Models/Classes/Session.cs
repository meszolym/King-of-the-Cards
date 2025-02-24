﻿using Newtonsoft.Json;
using System.Collections.Immutable;
using Timer = System.Timers.Timer;
using KC.App.Backend.Models.Classes.Hand;
using KC.App.Backend.Models.Structs;
using KC.App.Backend.Models.Interfaces;

namespace KC.App.Backend.Models.Classes;

public class Session(Guid id, ImmutableList<BettingBox> boxes, Shoe dealingShoe, DealerHand dealerHand, TickingTimer betPlacementTimer, TurnInfo turnInfo) : IIdentityBearer<Guid>
{
    public Guid Id { get; } = id;

    [JsonIgnore]
    public TickingTimer BetPlacementTimer { get; } = betPlacementTimer;
    public bool CanPlaceBets { get; set; } = true;
    public TurnInfo TurnInfo { get; set; } = turnInfo;
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime LastMoveAt { get; set; } = DateTime.Now;

    //properties moved from Table
    public ImmutableList<BettingBox> Boxes { get; } = boxes;

    [JsonIgnore]
    public Shoe DealingShoe { get; } = dealingShoe;

    public DealerHand DealerHand { get; } = dealerHand;
}