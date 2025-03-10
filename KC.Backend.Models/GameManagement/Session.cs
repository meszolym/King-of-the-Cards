using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameManagement;

namespace KC.Backend.Models.GameManagement;

public class Session(Table table, TickingTimer timer)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Table Table { get; } = table;
    public TurnInfo CurrentTurnInfo { get; set; } = new TurnInfo();
    public DateTime LastMoveMadeAt { get; set; } = DateTime.MinValue;
    public bool CanPlaceBets => BettingTimer.RemainingSeconds > 0;
    public TickingTimer BettingTimer { get; set; } = timer;
}