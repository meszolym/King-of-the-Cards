using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using KC.Frontend.Client.Models;
using KC.Frontend.Client.Utilities;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using ReactiveUI;
using RestSharp;

namespace KC.Frontend.Client.Services;

public static class Endpoints
{
    public static readonly Uri baseUri = new Uri("http://localhost:5238");

    //public static readonly Uri baseUri = new Uri("http://localhost:5000");
    public static readonly string Sessions = "session";
    public static readonly string Player = "player";
}
public class ExternalCommunicationException(string message) : Exception(message);

public class ExternalCommunicatorService
{
    private readonly RestClient _client = new RestClient(Endpoints.baseUri);

    private readonly Subject<bool> _connectionStatusSubject = new Subject<bool>();

    #region  Deprecated request handling
    
    // private bool _lastConnectionStatus = false;
    // public IObservable<bool> ConnectionStatus => _connectionStatusSubject;

    // private async Task<T> ExecuteRequest<T>(RestRequest request)
    // {
    //     var response = await _client.ExecuteAsync(request).ConfigureAwait(false);
    //
    //     if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    //     {
    //         _connectionStatusSubject.OnNext(false);
    //         _lastConnectionStatus = false;
    //     }
    //     else if (_lastConnectionStatus == false)
    //     {
    //         _connectionStatusSubject.OnNext(true);
    //     }
    //
    //     return response.IsSuccessful
    //         ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Content) ?? throw new ExternalCommunicationException("Could not deserialize response")
    //         : throw new ExternalCommunicationException($"Could not get data. Status: {response.StatusCode} - {response.StatusDescription} Error: {response.ErrorMessage}");
    // }
    //
    // private async Task ExecuteRequest(RestRequest request)
    // {
    //     var response = await _client.ExecuteAsync(request).ConfigureAwait(false);
    //     
    //     if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    //     {
    //         _connectionStatusSubject.OnNext(false);
    //         _lastConnectionStatus = false;
    //         
    //     }
    //     else if (_lastConnectionStatus == false)
    //     {
    //         _connectionStatusSubject.OnNext(true);
    //     }
    //
    //     if (!response.IsSuccessful)
    //     {
    //         throw new ExternalCommunicationException($"Status: {response.StatusCode} - {response.StatusDescription} Error: {response.ErrorMessage}");
    //     }
    // }
    
    #endregion
    

    public async Task<List<SessionListItem>> GetSessions()
    {
        var request = new RestRequest(Endpoints.Sessions);
        
        var sessions = await _client.GetAsync<List<SessionDto>>(request);
        
        return sessions is null
            ? throw new ExternalCommunicationException("Could not get sessions")
            : sessions.Select(s => new SessionListItem()
            {
                Id = s.Id, CurrentOccupancy = s.Table.BettingBoxes.Count(b => b.OwnerId != MacAddress.None), MaxOccupancy = s.Table.BettingBoxes.Count()
            }).ToList();
    }

    public async Task RegisterPlayer(string name)
    {
        var macAddress = ClientMacAddressHandler.GetMacAddress();
        var request = new RestRequest(Endpoints.Player).AddBody(new PlayerRegisterDto(name, macAddress));
        await _client.PostAsync(request);
    }

    public async Task<PlayerDto> GetPlayerByMac(MacAddress macAddress)
    {
        var request = new RestRequest(Endpoints.Player).AddBody(macAddress);
        var p = await _client.GetAsync<PlayerDto>(request);
        return p ?? throw new ExternalCommunicationException("Player not found");
    }
}

