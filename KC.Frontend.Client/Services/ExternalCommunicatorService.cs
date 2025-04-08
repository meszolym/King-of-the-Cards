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
    //public static readonly Uri BaseUri = new Uri("http://localhost:5000");
    
    public static readonly Uri SignalRHub = new Uri(BaseUri + "signalR");

    public static RestRequest GetSessions => new RestRequest(BaseUri + "session");
    public static RestRequest GetPlayerByMac => new RestRequest(BaseUri + "player/{macAddress}");
    public static RestRequest RegisterPlayer => new RestRequest(BaseUri + "player", Method.Post);
    public static RestRequest UpdatePlayerConnectionId => new RestRequest(BaseUri + "player", Method.Put);
    public static RestRequest ClaimBox => new RestRequest(BaseUri + "bettingbox", Method.Post);
    public static RestRequest DisclaimBox => new RestRequest(BaseUri + "bettingbox", Method.Delete);
    
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

    public bool SignalRInitialized { get; private set; } = false;
    
    public async Task ConnectToSignalR()
    {
        try
        {
            await SignalRHubConnection.StartAsync();
            SignalRInitialized = true;
            
        }
        catch (Exception e)
        {
            _connectionStatusSubject.OnNext(false);
            throw;
        }
        
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
        => await _client.GetAsync<PlayerReadDto?>(Endpoints.GetPlayerByMac.AddUrlSegment("macAddress", macAddress.Address)) 
           ?? throw new ExternalCommunicationException("Player not found");

    public async Task ClaimBox(Guid sessionId, int boxIdx, MacAddress primaryMacAddress) =>
        await _client.PostAsync(
            Endpoints.ClaimBox.AddBody(new BoxOwnerUpdateDto(sessionId, boxIdx, primaryMacAddress)));

    public async Task DisclaimBox(Guid sessionId, int boxIdx, MacAddress primaryMacAddress) =>
        await _client.DeleteAsync(
            Endpoints.DisclaimBox.AddBody(new BoxOwnerUpdateDto(sessionId, boxIdx, primaryMacAddress)));
}

