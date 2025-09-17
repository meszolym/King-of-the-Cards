using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameManagement;

namespace KC.Shared.Models.Misc;

public static class SignalRMethods
{
    public static SignalRMethod<SessionReadDto> SessionCreated { get; } = new("SessionCreated");
    public static SignalRMethod<Guid> SessionDeleted { get; } = new("SessionDeleted");
    public static SignalRMethod<PlayerReadDto> PlayerBalanceUpdated { get; } = new("PlayerBalanceUpdated");
    public static SignalRMethod<(Guid sessionId, int remainingSeconds)> BettingTimerTicked { get; } = new("BettingTimerTicked");
    public static SignalRMethod<Guid> BettingTimerElapsed { get; } = new("BettingTimerElapsed");
    public static SignalRMethod<Guid> BettingTimerStopped { get; } = new("BettingTimerStopped");
    public static SignalRMethod<SessionReadDto> BettingReset { get; } = new("BettingReset");
    public static SignalRMethod<SessionReadDto> HandsUpdated { get; } = new("HandsUpdated");
    public static SignalRMethod<BettingBoxReadDto> BetUpdated { get; } = new("BetUpdated");
    public static SignalRMethod<BettingBoxReadDto> BoxOwnerChanged { get; } = new("BoxOwnerChanged");
    public static SignalRMethod<SessionReadDto> SessionOccupancyChanged { get; } = new("SessionOccupancyChanged");
    public static SignalRMethod<TurnInfo> TurnChanged { get; } = new("TurnChanged");
    public static SignalRMethod<OutcomeReadDto> OutcomeCalculated { get; } = new("OutcomeCalculated");
    public static SignalRMethod<Guid> Shuffling { get; } = new("Shuffling");
}