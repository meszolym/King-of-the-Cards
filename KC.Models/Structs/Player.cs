namespace KC.Models.Structs;

public struct Player
{
    public string MacAddress { get; init; }
    public string Name { get; set; }
    public double Balance { get; set; }
}