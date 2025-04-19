using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameManagement;
using Timer = System.Timers.Timer;

namespace KC.Backend.Models.GameManagement;

public class Session(Table table, TimeSpan betTimerTimeSpan, TimeSpan destructionTimerTimeSpan)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Table Table { get; } = table;
    public TurnInfo CurrentTurnInfo { get; set; } = new TurnInfo();
    //public DateTime LastMoveMadeAt { get; set; } = DateTime.MinValue;

    public Timer DestructionTimer { get; set; } = new Timer(destructionTimerTimeSpan);
    public bool CanPlaceBets => BettingTimer.RemainingSeconds > 0;
    public TickingTimer BettingTimer { get; set; } = new TickingTimer(betTimerTimeSpan);
}