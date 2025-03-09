using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Frontend.Client.Models;
using KC.Shared.Models.Dtos;
using RestSharp;

namespace KC.Frontend.Client.Services;

public class ExternalCommunicatorService
{
    private readonly RestClient _client = new RestClient(Endpoints.baseUri);
    
    public async Task<List<SessionListItem>> GetSessions()
    {
        var request = new RestRequest(Endpoints.GetAllSessions);
        var response = await _client.GetAsync<List<SessionDto>>(request);
        return response == null ? new List<SessionListItem>() 
            : response.Select(s => new SessionListItem()
            {
                Id = s.Id,
                CurrentOccupancy = s.Table.BettingBoxes.Count(b => b.OwnerId != Guid.Empty),
                MaxOccupancy = s.Table.BettingBoxes.Count()
            }).ToList();

    }
    
}

public static class Endpoints
{
    public static readonly Uri baseUri = new Uri("http://localhost:37937");
    public static readonly string GetAllSessions = "sessions";
    
}