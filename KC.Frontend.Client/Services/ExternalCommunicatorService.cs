using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using KC.Frontend.Client.Models;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using RestSharp;

namespace KC.Frontend.Client.Services;

public class ExternalCommunicationException(string message) : Exception(message);

public class ExternalCommunicatorService
{
    private readonly RestClient _client = new RestClient(Endpoints.baseUri);

    public async Task<List<SessionListItem>> GetSessions()
    {
        var request = new RestRequest(Endpoints.GetAllSessions);
        var response = await _client.GetAsync<List<SessionDto>>(request);
        return response is null
            ? throw new ExternalCommunicationException("Could not get sessions")
            : response.Select(s => new SessionListItem()
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