namespace KC.App.Models.Structs;
public readonly record struct TurnInfo(Guid SessionId, int CurrentBoxIdx, int CurrentHandIdx);