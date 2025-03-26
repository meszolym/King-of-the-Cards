using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using KC.Frontend.Client.Models;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using ReactiveUI;
using RestSharp;

namespace KC.Frontend.Client.Services;

public class ExternalCommunicationException(string message) : Exception(message);

public class ExternalCommunicatorService
{
    private readonly RestClient _client = new RestClient(Endpoints.baseUri);

    private readonly Subject<bool> _connectionStatusSubject = new Subject<bool>();
    private bool _lastConnectionStatus = false;
    public IObservable<bool> ConnectionStatus => _connectionStatusSubject;
    
    private async Task<T> ExecuteRequest<T>(RestRequest request, Method method)
    {
        var response = await _client.ExecuteAsync(request, method).ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _connectionStatusSubject.OnNext(false);
            _lastConnectionStatus = false;
        }
        else if (_lastConnectionStatus == false)
        {
            _connectionStatusSubject.OnNext(true);
        }
        
        return response.IsSuccessful
            ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Content) ?? throw new ExternalCommunicationException("Could not deserialize response")
            : throw new ExternalCommunicationException("Could not get data");
    }
    
    public async Task<List<SessionListItem>> GetSessions()
    {
        var request = new RestRequest(Endpoints.GetAllSessions);
        
        var sessions = await ExecuteRequest<List<SessionDto>>(request, Method.Get).ConfigureAwait(false);
        return sessions is null
            ? throw new ExternalCommunicationException("Could not get sessions")
            : sessions.Select(s => new SessionListItem()
            {
                Id = s.Id,
                CurrentOccupancy = s.Table.BettingBoxes.Count(b => b.OwnerId != MacAddress.None),
                MaxOccupancy = s.Table.BettingBoxes.Count()
            }).ToList();
    }
    
    
}

public static class Endpoints
{
    public static readonly Uri baseUri = new Uri("http://localhost:5238");
    //public static readonly Uri baseUri = new Uri("http://localhost:5000");
    public static readonly string GetAllSessions = "session";
}