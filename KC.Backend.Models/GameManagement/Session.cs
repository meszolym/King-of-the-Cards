using KC.Backend.Models.GameItems;

namespace KC.Backend.Models.GameManagement;

public class Session(Table table, TickingTimer bettingTimer)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Table Table { get; } = table;
    public TurnInfo CurrentTurnInfo { get; set; } = new TurnInfo();
    public DateTime LastMoveMadeAt { get; set; } = DateTime.MinValue;
    public bool CanPlaceBets => BettingTimer.RemainingSeconds > 0;
    public TickingTimer BettingTimer { get; set; } = bettingTimer;
}