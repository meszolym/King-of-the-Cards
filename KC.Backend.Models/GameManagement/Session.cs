using KC.Backend.Models.Utils.Interfaces;

namespace KC.Backend.Models.GameManagement;

public class Session(Table table) : IIdentityBearer<Guid>
{
    public Guid Id { get; } = Guid.NewGuid();
    public Table Table { get; } = table;
    public TurnInfo CurrentTurnInfo { get; set; } = TurnInfo.None;
    public bool CanPlaceBets { get; set; } = false;

}