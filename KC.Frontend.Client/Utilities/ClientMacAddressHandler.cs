using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

    // private static MacAddress GetPrimaryMacAddress()
    // {
    //     var macBytes = NetworkInterface.GetAllNetworkInterfaces()
    //         .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
    //         .OrderBy(nic => nic.GetIPProperties().GetIPv4Properties()?.Index ?? int.MaxValue) // Sort by priority
    //         .Select(nic => nic.GetPhysicalAddress().GetAddressBytes())
    //         .FirstOrDefault();
    //
    //     if (macBytes == null || macBytes.Length == 0)
    //         throw new InvalidOperationException("No active network adapter found.");
    //
    //     // Convert bytes to a MAC address string with ":" separator
    //     var macString = string.Join(":", macBytes.Select(b => b.ToString("X2")));
    //
    //     return macString is not null ? new MacAddress(macString) : throw new NetworkAdapterException("Could not find network adapter");
    // }
    
    private static MacAddress GetPrimaryMacAddress()
    {
        IPAddress localIP;

        // Step 1: Determine the local IP address used to reach the internet
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530); // Google's public DNS - no actual data sent
            localIP = ((IPEndPoint)socket.LocalEndPoint).Address;
        }

        // Step 2: Find the NIC that has this IP assigned
        var nic = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(n =>
                n.OperationalStatus == OperationalStatus.Up &&
                n.GetIPProperties().UnicastAddresses
                    .Any(a => a.Address.Equals(localIP)));

        if (nic == null)
            throw new InvalidOperationException("No network adapter found for internet connection.");

        var macBytes = nic.GetPhysicalAddress().GetAddressBytes();
        if (macBytes == null || macBytes.Length == 0)
            throw new InvalidOperationException("MAC address not found for the primary network adapter.");

        var macString = string.Join(":", macBytes.Select(b => b.ToString("X2")));
        return new MacAddress(macString);
    }
}