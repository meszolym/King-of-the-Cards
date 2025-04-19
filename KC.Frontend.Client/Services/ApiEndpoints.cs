using System;
using RestSharp;

namespace KC.Frontend.Client.Services;

public static class ApiEndpoints
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
    public static RestRequest CreateSession => new(BaseUri + "session/create", Method.Post);
}