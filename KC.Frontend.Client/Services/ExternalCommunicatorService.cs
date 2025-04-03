using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KC.Frontend.Client.Models;
using KC.Frontend.Client.Utilities;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace KC.Frontend.Client.Services;

public static class Endpoints
{
    public static readonly Uri BaseUri = new Uri("http://localhost:5238");

    //public static readonly Uri baseUri = new Uri("http://localhost:5000");
    public static readonly RestRequest GetSessions = new RestRequest(BaseUri + "session");
    public static readonly RestRequest GetPlayerByMac = new RestRequest(BaseUri + "player/{macAddress}");
    public static readonly RestRequest RegisterPlayer = new RestRequest(BaseUri + "player", Method.Post);
    public static readonly RestRequest UpdatePlayerConnectionId = new RestRequest(BaseUri + "player", Method.Put);
    
    public static readonly string SignalRHub = BaseUri + "signalR";
}
public class ExternalCommunicationException(string message) : Exception(message);

public class ExternalCommunicatorService
{
    private readonly RestClient _client = new RestClient(Endpoints.BaseUri,
        configureSerialization: s => s.UseNewtonsoftJson());

    public readonly HubConnection SignalRHubConnection = new HubConnectionBuilder().WithUrl(Endpoints.SignalRHub).WithAutomaticReconnect().Build();
    
    private readonly Subject<bool> _connectionStatusSubject = new Subject<bool>();
    public IObservable<bool> ConnectionStatus => _connectionStatusSubject;
    
    #region  Deprecated request handling
    
    // private bool _lastConnectionStatus = false;


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

    public ExternalCommunicatorService()
    {
        SignalRHubConnection.Reconnecting += _ =>
        {
            _connectionStatusSubject.OnNext(false);
            return Task.CompletedTask;
        }; 
        
        SignalRHubConnection.Reconnected += _ =>
        {
            _connectionStatusSubject.OnNext(true);
            return Task.CompletedTask;
        };
    }
    
    public async Task ConnectToSignalR()
    {
        await SignalRHubConnection.StartAsync();
        if (SignalRHubConnection.State == HubConnectionState.Connected)
        {
            _connectionStatusSubject.OnNext(true);
            return;
        }
        _connectionStatusSubject.OnNext(false);
    }

    public async Task UpdatePlayerConnectionId()
    {
        if (SignalRHubConnection.State != HubConnectionState.Connected)
        {
            throw new ExternalCommunicationException("SignalR connection is not established");
        }
        await _client.PutAsync(Endpoints.UpdatePlayerConnectionId.AddBody(new PlayerConnectionIdUpdateDto(ClientMacAddressHandler.PrimaryMacAddress, SignalRHubConnection.ConnectionId!)));
    }

    public async Task<IEnumerable<SessionListItem>> GetSessions()
        => (await _client.GetAsync<List<SessionReadDto>>(Endpoints.GetSessions)
            ?? throw new ExternalCommunicationException("Could not get sessions"))
           .Select(s => new SessionListItem()
           {
               Id = s.Id, CurrentOccupancy = s.Table.BettingBoxes.Count(b => b.OwnerId != MacAddress.None), MaxOccupancy = s.Table.BettingBoxes.Count()
           });

    public async Task RegisterPlayer(string name) => await _client.PostAsync(Endpoints.RegisterPlayer.AddBody(new PlayerRegisterDto(name, ClientMacAddressHandler.PrimaryMacAddress)));

    public async Task<PlayerReadDto> GetPlayerByMac(MacAddress macAddress) 
        => await _client.GetAsync<PlayerReadDto>(Endpoints.GetPlayerByMac.AddUrlSegment("macAddress", macAddress.Address)) 
           ?? throw new ExternalCommunicationException("Player not found");
}

