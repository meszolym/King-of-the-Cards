using System;
using System.Linq;
using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Frontend.Client.Utilities;

public class NetworkAdapterException(string message) : Exception(message);

public static class ClientMacAddressHandler
{
    private static bool _isMacAddressSet = false;
    private static MacAddress _macAddress;
    public static MacAddress PrimaryMacAddress
    {
        get
        {
            if (_isMacAddressSet) return _macAddress;
            
            _macAddress = GetPrimaryMacAddress();
            _isMacAddressSet = true;
            return _macAddress;
        }
    }
    public static MacAddress GetPrimaryMacAddress()
    {
        var macBytes = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .OrderBy(nic => nic.GetIPProperties().GetIPv4Properties()?.Index ?? int.MaxValue) // Sort by priority
            .Select(nic => nic.GetPhysicalAddress().GetAddressBytes())
            .FirstOrDefault();

        if (macBytes == null || macBytes.Length == 0)
            throw new InvalidOperationException("No active network adapter found.");

        // Convert bytes to a MAC address string with ":" separator
        var macString = string.Join(":", macBytes.Select(b => b.ToString("X2")));

        return macString is not null ? new MacAddress(macString) : throw new NetworkAdapterException("Could not find network adapter");
    }
}