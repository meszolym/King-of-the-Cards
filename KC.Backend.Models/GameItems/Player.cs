using KC.Backend.Models.Utils.Interfaces;

namespace KC.Backend.Models;

public class Player( string name, double balance, string connectionId) : IIdentityBearer<Guid>
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string ConnectionId { get; set; } = connectionId;
    public string Name { get; set; } = name;
    public double Balance { get; set; } = balance;

    public static Player None => new Player("None", 0, "None") { Id = Guid.Empty };
}