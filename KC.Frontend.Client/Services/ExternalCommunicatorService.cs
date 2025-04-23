using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Models;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace KC.Frontend.Client.Services;

public class ExternalCommunicationException(string message) : Exception(message);

public partial class ExternalCommunicatorService
{
    private readonly RestClient _client = new(ApiEndpoints.BaseUri,
        configureSerialization: s => s.UseNewtonsoftJson());

    private readonly HubConnection _signalRHubConnection =
        new HubConnectionBuilder().WithUrl(ApiEndpoints.SignalRHub).WithAutomaticReconnect().AddNewtonsoftJsonProtocol().Build();

    private readonly Subject<bool> _connectionStatusSubject = new();
    public IObservable<bool> ConnectionStatus => _connectionStatusSubject;

    public ExternalCommunicatorService()
    {
        InitSignalRStatuses();
        SignalREvents.Init(_signalRHubConnection);
    }

    private void InitSignalRStatuses()
    {
        _signalRHubConnection.Reconnecting += _ =>
        {
            _connectionStatusSubject.OnNext(false);
            return Task.CompletedTask;
        };

        _signalRHubConnection.Reconnected += _ =>
        {
            _connectionStatusSubject.OnNext(true);
            return Task.CompletedTask;
        };
        
        _signalRHubConnection.Closed += _ =>
        {
            _connectionStatusSubject.OnNext(false);
            return Task.CompletedTask;
        };
    }
    
    public bool SignalRInitialized { get; private set; }

    public async Task ConnectToSignalR()
    {
        try
        {
            await _signalRHubConnection.StartAsync();
            SignalRInitialized = true;
        }
        catch (Exception e)
        {
            _connectionStatusSubject.OnNext(false);
            throw;
        }

        if (_signalRHubConnection.State == HubConnectionState.Connected)
        {
            _connectionStatusSubject.OnNext(true);
            return;
        }

        _connectionStatusSubject.OnNext(false);
    }

    public async Task UpdatePlayerConnectionId(MacAddress macAddress)
    {
        if (_signalRHubConnection.State != HubConnectionState.Connected)
            throw new ExternalCommunicationException("SignalR connection is not established");

        await _client.PutAsync(ApiEndpoints.UpdatePlayerConnectionId.AddBody(
            new PlayerConnectionIdUpdateDto(macAddress,
                _signalRHubConnection.ConnectionId!)));
    }

    public async Task<IEnumerable<SessionListItem>> GetSessionList()
    {
        return (await _client.GetAsync<List<SessionReadDto>>(ApiEndpoints.GetSessions)
                ?? throw new ExternalCommunicationException("Could not get sessions"))
            .Select(s => s.ToSessionListItem());
    }

    public async Task<SessionReadDto> GetSession(Guid sessionId) =>
        await _client.GetAsync<SessionReadDto>(
            ApiEndpoints.GetSession.AddUrlSegment("id", sessionId.ToString()))
        ?? throw new ExternalCommunicationException("Session not found");

    public async Task RegisterPlayer(string name, MacAddress macAddress)
    {
        await _client.PostAsync(
            ApiEndpoints.RegisterPlayer.AddBody(new PlayerRegisterDto(name, macAddress)));
    }

    public async Task<PlayerReadDto> GetPlayerByMac(MacAddress macAddress)
    {
        return await _client.GetAsync<PlayerReadDto?>(
                   ApiEndpoints.GetPlayerByMac.AddUrlSegment("macAddress", macAddress.Address))
               ?? throw new ExternalCommunicationException("Player not found");
    }

    public async Task ClaimBox(Guid sessionId, int boxIdx, MacAddress primaryMacAddress)
    {
        await _client.PostAsync(
            ApiEndpoints.ClaimBox.AddBody(new BoxOwnerUpdateDto(sessionId, boxIdx, primaryMacAddress)));
    }

    public async Task DisclaimBox(Guid sessionId, int boxIdx, MacAddress primaryMacAddress)
    {
        await _client.DeleteAsync(
            ApiEndpoints.DisclaimBox.AddBody(new BoxOwnerUpdateDto(sessionId, boxIdx, primaryMacAddress)));
    }

    public async Task UpdateBet(Guid sessionId, int boxIdx, MacAddress primaryMacAddress, double amount, int handIdx = 0)
    {
        await _client.PutAsync(
            ApiEndpoints.UpdateBet.AddBody(new BoxBetUpdateDto(sessionId, boxIdx, primaryMacAddress, amount, handIdx)));
    }
    
    public async Task JoinSession(Guid sessionId, MacAddress macAddress)
    {
        await _client.PostAsync(ApiEndpoints.JoinSession.AddBody(new SessionJoinLeaveDto(sessionId, macAddress)));
    }

    public async Task LeaveSession(Guid sessionId, MacAddress macAddress)
    {
        await _client.DeleteAsync(ApiEndpoints.LeaveSession.AddBody(new SessionJoinLeaveDto(sessionId, macAddress)));
    }
    
    public async Task CreateSession() => await _client.PostAsync(ApiEndpoints.CreateSession);
}