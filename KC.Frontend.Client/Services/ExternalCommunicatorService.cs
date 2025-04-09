using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KC.Frontend.Client.Models;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace KC.Frontend.Client.Services;

public static class Endpoints
{
    public static readonly Uri BaseUri = new("http://localhost:5238");
    //public static readonly Uri BaseUri = new Uri("http://localhost:5000");

    public static readonly Uri SignalRHub = new(BaseUri + "signalR");

    public static RestRequest GetSessions => new(BaseUri + "session");
    public static RestRequest GetSession => new(BaseUri + "session/{id}");
    public static RestRequest GetPlayerByMac => new(BaseUri + "player/{macAddress}");
    public static RestRequest RegisterPlayer => new(BaseUri + "player", Method.Post);

    public static RestRequest UpdatePlayerConnectionId => new(BaseUri + "player/update-conn-id", Method.Put);

    public static RestRequest ClaimBox => new(BaseUri + "bettingbox/claim-box", Method.Post);
    public static RestRequest DisclaimBox => new(BaseUri + "bettingbox/disclaim-box", Method.Delete);
    public static RestRequest JoinSession => new(BaseUri + "session/join", Method.Post);
    public static RestRequest LeaveSession => new(BaseUri + "session/leave", Method.Delete);
}

public class ExternalCommunicationException(string message) : Exception(message);

public class ExternalCommunicatorService
{
    private readonly RestClient _client = new(Endpoints.BaseUri,
        configureSerialization: s => s.UseNewtonsoftJson());

    public readonly HubConnection SignalRHubConnection =
        new HubConnectionBuilder().WithUrl(Endpoints.SignalRHub).WithAutomaticReconnect().Build();

    private readonly Subject<bool> _connectionStatusSubject = new();
    public IObservable<bool> ConnectionStatus => _connectionStatusSubject;

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

    public bool SignalRInitialized { get; private set; }

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

    public async Task UpdatePlayerConnectionId(MacAddress macAddress)
    {
        if (SignalRHubConnection.State != HubConnectionState.Connected)
            throw new ExternalCommunicationException("SignalR connection is not established");

        await _client.PutAsync(Endpoints.UpdatePlayerConnectionId.AddBody(
            new PlayerConnectionIdUpdateDto(macAddress,
                SignalRHubConnection.ConnectionId!)));
    }

    public async Task<IEnumerable<SessionListItem>> GetSessionList()
    {
        return (await _client.GetAsync<List<SessionReadDto>>(Endpoints.GetSessions)
                ?? throw new ExternalCommunicationException("Could not get sessions"))
            .Select(s => new SessionListItem
            {
                Id = s.Id, CurrentOccupancy = s.Table.BettingBoxes.Count(b => b.OwnerId != MacAddress.None),
                MaxOccupancy = s.Table.BettingBoxes.Count()
            });
    }

    public async Task<SessionReadDto> GetSession(Guid sessionId) =>
        await _client.GetAsync<SessionReadDto>(
            Endpoints.GetSession.AddUrlSegment("id", sessionId.ToString()))
        ?? throw new ExternalCommunicationException("Session not found");

    public async Task RegisterPlayer(string name, MacAddress macAddress)
    {
        await _client.PostAsync(
            Endpoints.RegisterPlayer.AddBody(new PlayerRegisterDto(name, macAddress)));
    }

    public async Task<PlayerReadDto> GetPlayerByMac(MacAddress macAddress)
    {
        return await _client.GetAsync<PlayerReadDto?>(
                   Endpoints.GetPlayerByMac.AddUrlSegment("macAddress", macAddress.Address))
               ?? throw new ExternalCommunicationException("Player not found");
    }

    public async Task ClaimBox(Guid sessionId, int boxIdx, MacAddress primaryMacAddress)
    {
        await _client.PostAsync(
            Endpoints.ClaimBox.AddBody(new BoxOwnerUpdateDto(sessionId, boxIdx, primaryMacAddress)));
    }

    public async Task DisclaimBox(Guid sessionId, int boxIdx, MacAddress primaryMacAddress)
    {
        await _client.DeleteAsync(
            Endpoints.DisclaimBox.AddBody(new BoxOwnerUpdateDto(sessionId, boxIdx, primaryMacAddress)));
    }

    public async Task JoinSession(Guid sessionId, MacAddress macAddress)
    {
        await _client.PostAsync(Endpoints.JoinSession.AddBody(new SessionJoinLeaveDto(sessionId, macAddress)));
    }

    public async Task LeaveSession(Guid sessionId, MacAddress macAddress)
    {
        await _client.DeleteAsync(Endpoints.LeaveSession.AddBody(new SessionJoinLeaveDto(sessionId, macAddress)));
    }
}