namespace KC.Models.Structs;

public class Player(string macAddress, string name, double balance)
{
    public string MacAddress { get; init; } = macAddress;
    public string Name { get; set; } = name;
    public double Balance { get; set; } = balance;
}