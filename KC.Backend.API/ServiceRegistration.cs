using System.Diagnostics;
using KC.Backend.API.Extensions;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic;
using KC.Backend.Logic.Logics;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services.Interfaces;
using KC.Shared.Models.Misc;

namespace KC.Backend.API;

public delegate Task OnTurnInfoChangedDelegate(Guid sessionId);
public delegate Task OnBettingTimerTickedDelegate(Guid sessionId, int remainingSeconds);
public delegate Task OnBettingTimerElapsedDelegate(Guid sessionId);
public delegate Task OnDestructionTimerElapsedDelegate(Guid sessionId);

public static class ServiceRegistration
{
    public static IServiceCollection RegisterApiLayerServices(this IServiceCollection services)
    {
        Dictionary<string, string> connectionsAndGroups = [];
        services.AddSingleton<IDictionary<string, string>>(connectionsAndGroups);
        services.AddTransient<IClientCommunicator, ClientCommunicator>();
            
        services.AddSignalR().AddNewtonsoftJsonProtocol();
        
        services.AddTransient<IMoveOrchestrator, MoveOrchestrator>();
        
        return services;
    }
    
    private const int DelaySecsBetweenCards = 2;
    
    public static IServiceCollection RegisterDelegates(this IServiceCollection services)
    {
        //OnTurnInfoChanged
        services.AddSingleton<OnTurnInfoChangedDelegate>(s =>
        {
            var sessionLogic = s.GetRequiredService<ISessionLogic>();
            var hub = s.GetRequiredService<IClientCommunicator>();
            var gamePlayLogic = s.GetRequiredService<IGamePlayLogic>();
            var playerLogic = s.GetRequiredService<IPlayerLogic>();
            var betUpdated = s.GetRequiredService<BetUpdatedDelegate>();
            
            return async sessId =>
            {
                var session = sessionLogic.Get(sessId);
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.TurnChanged, session.CurrentTurnInfo);
                if (session.CurrentTurnInfo.PlayersTurn) return;

                await gamePlayLogic.DealerPlayHand(sessId, TimeSpan.FromSeconds(DelaySecsBetweenCards));

                //TODO: Check winners, pay out bets, clear hands
                gamePlayLogic.FinishAllHandsInPlay(sessId);
                
                await gamePlayLogic.PayOutBetsToBettingBoxes(sessId);
                
                //Pay out to players from the betting boxes
                foreach (var b in session.Table.BettingBoxes)
                {
                    if (b.OwnerId == Guid.Empty) continue;
                    var player = playerLogic.Get(b.OwnerId);
                    playerLogic.AddToBalance(b.OwnerId, b.Hands.Sum(h => h.Bet));
                    await hub.SendMessageAsync(player.ConnectionId, SignalRMethods.PlayerBalanceUpdated, player.ToDto());
                    await betUpdated(sessId, b.IdxOnTable);
                }
                
                await gamePlayLogic.ClearHands(sessId);

            };
        });
        
        //OnBettingTimerTicked
        services.AddSingleton<OnBettingTimerTickedDelegate>(s =>
        {
            var hub = s.GetRequiredService<IClientCommunicator>();
            return async (Guid sessionId, int remainingSeconds) => await hub.SendMessageToGroupAsync(sessionId,
                SignalRMethods.BettingTimerTicked, (sessionId, remainingSeconds));
        });
        
        //OnBettingTimerElapsed
        services.AddSingleton<OnBettingTimerElapsedDelegate>(s =>
        {
            var hub = s.GetRequiredService<IClientCommunicator>();
            var gamePlayLogic = s.GetRequiredService<IGamePlayLogic>();
            var sessionLogic = s.GetRequiredService<ISessionLogic>();
            var getPlayerName = s.GetRequiredService<GetPlayerNameDelegate>();

            return async sessId =>
            {
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.BettingTimerElapsed, sessId);

                gamePlayLogic.ShuffleIfNeeded(sessId);
        
                var session = sessionLogic.Get(sessId);
                await gamePlayLogic.DealStartingCards(sessId, TimeSpan.FromSeconds(DelaySecsBetweenCards));
                var dealerBj = gamePlayLogic.DealerCheck(sessId);
                if (dealerBj)
                {
                    gamePlayLogic.FinishAllHandsInPlay(sessId);
                    await gamePlayLogic.PayOutBetsToBettingBoxes(sessId); //Informs players of their bets
                    //This ClearHands deletes hands, so that means the bets are cleared too TODO: get the bets from the boxes to the player balance
                    await gamePlayLogic.ClearHands(sessId);
                    
                    //TODO: Handle the rest of this case, like resetting the session timer and stuff
                    return;
                }
                await gamePlayLogic.TransferTurn(sessId);
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.TurnChanged, session.CurrentTurnInfo);
            };
        });

        //OnDestructionTimerElapsed
        services.AddSingleton<OnDestructionTimerElapsedDelegate>(s =>
        {
            var sessionTerminatorService = s.GetRequiredService<ISessionTerminatorService>();
            var hub = s.GetRequiredService<IClientCommunicator>();
            var playerLogic = s.GetRequiredService<IPlayerLogic>();

            return async id =>
            {
                var refundedPlayers = sessionTerminatorService.RefundAndRemoveSession(id).Select(playerLogic.Get)
                    .ToList();

                var connected = hub.ConnectionsAndGroups.Where(x => x.Value == id.ToString()).ToList();
                foreach (var conn in connected)
                {
                    await hub.MoveToGroupAsync(conn.Key, hub.BaseGroup);
                }

                await hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionDeleted, id);

                foreach (var p in refundedPlayers)
                {
                    await hub.SendMessageAsync(p.ConnectionId, SignalRMethods.PlayerBalanceUpdated, p.ToDto());
                }
            };
        });
        
        //HandUpdated -> Defined in Logic.ServiceRegistration
        services.AddSingleton<HandUpdatedDelegate>(s =>
        {
            var hub = s.GetRequiredService<IClientCommunicator>();
            var sessionLogic = s.GetRequiredService<ISessionLogic>();
            var getPlayerName = s.GetRequiredService<GetPlayerNameDelegate>();
            
            return async sessionId =>
            {
                var session = sessionLogic.Get(sessionId);
                await hub.SendMessageToGroupAsync(sessionId, SignalRMethods.HandsUpdated, session.ToDto(getPlayerName));
            };
            
        });
        
        //BetUpdated -> Defined in Logic.ServiceRegistration
        services.AddSingleton<BetUpdatedDelegate>(s =>
        {
            var hub = s.GetRequiredService<IClientCommunicator>();
            var sessionLogic = s.GetRequiredService<ISessionLogic>();
            var getPlayerName = s.GetRequiredService<GetPlayerNameDelegate>();
            
            return async (sessionId, boxIdx) =>
            {
                var session = sessionLogic.Get(sessionId);
                await hub.SendMessageToGroupAsync(sessionId, SignalRMethods.BetUpdated, session.Table.BettingBoxes[boxIdx].ToDto(getPlayerName));
            };
        });
        
        return services;
    }
}