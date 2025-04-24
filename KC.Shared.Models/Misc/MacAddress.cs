using System.Text.RegularExpressions;

namespace KC.Shared.Models.Misc;

public readonly partial struct MacAddress(string address) : IEquatable<MacAddress>
{
    public static MacAddress Parse(string address) => new MacAddress(address);
    public readonly string Address { get; } = MacAddressRegex().IsMatch(address) ? address : throw new ArgumentException("Address is not standard", nameof(address));

    [GeneratedRegex("^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$")]
    private static partial Regex MacAddressRegex();

    public static readonly MacAddress None = new MacAddress("00:00:00:00:00:00");
    
    public static bool operator ==(MacAddress left, MacAddress right) => left.Address.Equals(right.Address, StringComparison.OrdinalIgnoreCase);
    public static bool operator !=(MacAddress left, MacAddress right) => !(left == right);
    
    public override bool Equals(object? obj) => obj is MacAddress address && address == this;
    public override int GetHashCode() => Address.GetHashCode();
    
    public override string ToString() => Address;

    public bool Equals(MacAddress other)
    {
        return Address == other.Address;
    }
}