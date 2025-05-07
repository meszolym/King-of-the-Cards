using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameManagement;
using Timer = System.Timers.Timer;

namespace KC.Backend.Models.GameManagement;

public class SessionBase(Table table)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Table Table { get; } = table;
    public bool CanPlaceBets { get; protected set; }
    protected TurnInfo TurnInfo;
    public TurnInfo CurrentTurnInfo => TurnInfo;
}