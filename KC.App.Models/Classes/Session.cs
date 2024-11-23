using KC.App.Models.Interfaces;
using KC.App.Models.Records;
using KC.App.Models.Structs;
using Timer = System.Timers.Timer;

namespace KC.App.Models.Classes;

public class Session(Guid id, Table table, Timer betPlacementTimer, TurnInfo turnInfo) : IIdentityBearer<Guid>
{
    public Guid Id { get; } = id;
    public Table Table { get; } = table;
    public Timer BetPlacementTimer { get; } = betPlacementTimer;
    public bool CanPlaceBets { get; set; } = true;
    public TurnInfo TurnInfo { get; set; } = turnInfo;
}