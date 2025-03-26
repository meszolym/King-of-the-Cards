using System;
using System.Linq;
using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Frontend.Client.Utilities;

public class NetworkAdapterException(string message) : Exception(message);

public static class ClientMacAddressHandler
{
    public static MacAddress GetMacAddress()
    {
        var macString = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .OrderBy(nic => nic.GetIPProperties().GetIPv4Properties()?.Index ?? int.MaxValue) // Sort by priority
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault();

        return macString is not null ? new MacAddress(macString) : throw new NetworkAdapterException("Could not find network adapter");
    }
}