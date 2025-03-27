using System.Text.RegularExpressions;

namespace KC.Shared.Models.Misc;

public partial struct MacAddress
{
    public MacAddress(string address)
    {
        Address = MacAddressRegex().IsMatch(address) ? address : throw new ArgumentException("Address is not standard", nameof(address));
    }
    public readonly string Address { get; }

    [GeneratedRegex("^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$")]
    private static partial Regex MacAddressRegex();

    public static MacAddress None = new MacAddress("00:00:00:00:00:00");
    
    public static bool operator ==(MacAddress left, MacAddress right) => left.Address.Equals(right.Address, StringComparison.OrdinalIgnoreCase);
    public static bool operator !=(MacAddress left, MacAddress right) => !(left == right);
    
    public override bool Equals(object? obj) => obj is MacAddress address && address == this;
    public override int GetHashCode() => Address.GetHashCode();
}