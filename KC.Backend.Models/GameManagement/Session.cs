using System.ComponentModel;
using System.Runtime.CompilerServices;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameManagement;
using Timer = System.Timers.Timer;

namespace KC.Backend.Models.GameManagement;

public sealed class Session(Table table, TimeSpan betTimerTimeSpan, TimeSpan destructionTimerTimeSpan)
    : SessionBase(table)
{
    /// <summary>
    /// Shows when it is time to destroy the session as there was no activity.
    /// </summary>
    public Timer DestructionTimer { get; set; } = new(destructionTimerTimeSpan);

    public TickingTimer BettingTimer { get; set; } = new(betTimerTimeSpan);

    public new bool CanPlaceBets => BettingTimer.RemainingSeconds > 0;
    
    public new TurnInfo CurrentTurnInfo
    {
        get => TurnInfo;
        set
        {
            if (EqualityComparer<TurnInfo>.Default.Equals(TurnInfo, value)) return;
            TurnInfo = value;
            TurnInfoChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTurnInfo)));
        }
    }
    public event PropertyChangedEventHandler? TurnInfoChanged;
}