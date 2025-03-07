namespace KC.Backend.Models;

public class Player( string name, double balance, string connectionId)
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string ConnectionId { get; set; } = connectionId;
    public string Name { get; set; } = name;
    public double Balance { get; set; } = balance;
}