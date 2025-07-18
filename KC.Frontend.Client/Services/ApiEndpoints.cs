using System;
using RestSharp;

namespace KC.Frontend.Client.Services;

public partial class ExternalCommunicatorService
{
    private static class ApiEndpoints
    {
        public const string SignalRBasePath = "/signalR";

        //Player
        
        /// <summary>
        /// Gets the player by the MAC address in the header.
        /// </summary>
        public static RestRequest GetPlayerByMacInHeader => new("/player");
        
        /// <summary>
        /// Registers a player with the given name and MAC address in the header.
        /// </summary>
        public static RestRequest RegisterPlayer => new("/player/{name}", Method.Post);
        
        /// <summary>
        /// Updates the player's SignalR connection ID with the given connection ID and MAC address in the header.
        /// </summary>
        public static RestRequest UpdatePlayerConnectionId => new("/player/update-conn-id/{connectionId}", Method.Put);
        
        //Session
        
        /// <summary>
        /// Gets all sessions.
        /// </summary>
        public static RestRequest GetSessions => new("/session");
        
        /// <summary>
        /// Gets a specific session by its ID.
        /// </summary>
        public static RestRequest GetSession => new("/session/{id}");
        
        /// <summary>
        /// Joins a session with the given session ID and MAC address in the header.
        /// </summary>
        public static RestRequest JoinSession => new("/session/join/{sessionId}", Method.Post);
        
        /// <summary>
        /// Leaves a session with the given session ID and MAC address in the header.
        /// </summary>
        public static RestRequest LeaveSession => new("/session/leave/{sessionId}", Method.Delete);
        
        /// <summary>
        /// Creates a new session.
        /// </summary>
        public static RestRequest CreateSession => new("/session/create", Method.Post);
        
        //BettingBox
        
        /// <summary>
        /// Claims a betting box with the appropriate info in the body and MAC address in the header.
        /// </summary>
        public static RestRequest ClaimBox => new("/bettingbox/claim-box", Method.Post);
        
        /// <summary>
        /// Disclaims a betting box with the appropriate info in the body and MAC address in the header.
        /// </summary>
        public static RestRequest DisclaimBox => new("/bettingbox/disclaim-box", Method.Delete);
        
        /// <summary>
        /// Updates a bet with the appropriate info in the body and MAC address in the header.
        /// </summary>
        public static RestRequest UpdateBet => new("/bettingbox/update-bet", Method.Put);
        
        //Gameplay
        
        /// <summary>
        /// Gets the possible moves for a hand.
        /// </summary>
        public static RestRequest GetPossibleMovesOnHand => new("/gameplay/get-moves/{sessionId}/{boxIdx}/{handIdx}");
        
        /// <summary>
        /// Makes move on hand with the appropriate info in the body and MAC address in the header.
        /// </summary>
        public static RestRequest MakeMoveOnHand => new("/gameplay/make-move", Method.Post);
        
        public static RestRequest ResetPlayerBalance => new("/player/reset-money", Method.Post);
        
    }
}
