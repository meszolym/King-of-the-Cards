using KC.Models.Interfaces;

namespace KC.Models.Classes;

public class Player(string id, string name, double balance) : IIdentityBearer<string>
{
    public string Id { get; init; } = id;
    public string Name { get; set; } = name;
    public double Balance { get; set; } = balance;
}