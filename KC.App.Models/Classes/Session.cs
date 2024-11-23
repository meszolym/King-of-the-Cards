using KC.App.Models.Interfaces;
using KC.App.Models.Records;
using Timer = System.Timers.Timer;

namespace KC.App.Models.Classes;

public class Session(Guid id, Table table, Timer betPlacementTimer) : IIdentityBearer<Guid>
{
    public Guid Id { get; } = id;
    public Table Table { get; } = table;
    public Timer BetPlacementTimer { get; } = betPlacementTimer;
    public bool CanPlaceBets { get; set; } = true;
    public int? CurrentBoxIdx { get; set; } = null; //invalid index
    public int? CurrentHandIdx { get; set; } = null; //invalid index
}