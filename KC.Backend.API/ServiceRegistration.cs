using System.Diagnostics;
using KC.Backend.API.Extensions;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic;
using KC.Backend.Logic.Logics;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services.Interfaces;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameItems;
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
    
    public static IServiceCollection RegisterDelegates(this IServiceCollection services)
    {
        //Shuffle -> Defined in Logic.ServiceRegistration
        services.AddSingleton<ShuffleDelegate>(s =>
        {
            var hub = s.GetRequiredService<IClientCommunicator>();
            return async sessionId => await hub.SendMessageToGroupAsync(sessionId, SignalRMethods.Shuffling,sessionId);
        });
        
        //OnOutcomeCalculated -> Defined in Logic.ServiceRegistration
        services.AddSingleton<OutcomeCalculatedDelegate>(s =>
        {
            var hub = s.GetRequiredService<IClientCommunicator>();
            
            return async (sessionId, boxIdx, handIdx, outcome)
                => await hub.SendMessageToGroupAsync(sessionId, SignalRMethods.OutcomeCalculated, 
                    new OutcomeReadDto(sessionId, boxIdx, handIdx, outcome));
        });
        
        //OnTurnInfoChanged
        services.AddSingleton<OnTurnInfoChangedDelegate>(s =>
        {
            var sessionLogic = s.GetRequiredService<ISessionLogic>();
            var hub = s.GetRequiredService<IClientCommunicator>();
            var gamePlayLogic = s.GetRequiredService<IGamePlayLogic>();
            var playerLogic = s.GetRequiredService<IPlayerLogic>();
            var betUpdated = s.GetRequiredService<BetUpdatedDelegate>();
            var getPlayerName = s.GetRequiredService<GetPlayerNameDelegate>();
            
            return async sessId =>
            {
                var session = sessionLogic.Get(sessId);
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.TurnChanged, session.CurrentTurnInfo);
                if (session.CurrentTurnInfo.PlayersTurn) return;

                await gamePlayLogic.DealerPlayHand(sessId);
                
                gamePlayLogic.FinishAllHandsInPlay(sessId);
                
                await gamePlayLogic.PayOutBetsToBettingBoxes(sessId);
                await Task.Delay(Constants.BetweenCardsDelayMs);
                
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
                sessionLogic.ZeroBettingTimer(sessId);
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.BettingReset, session.ToDto(getPlayerName));
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
            var playerLogic = s.GetRequiredService<IPlayerLogic>();
            var betUpdated = s.GetRequiredService<BetUpdatedDelegate>();

            return async sessId =>
            {
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.BettingTimerElapsed, sessId);

                gamePlayLogic.ShuffleIfNeeded(sessId); //First shuffle of the game will not be announced
        
                var session = sessionLogic.Get(sessId);
                await gamePlayLogic.DealStartingCards(sessId);
                var dealerBj = gamePlayLogic.DealerCheck(sessId);
                if (dealerBj)
                {
                    gamePlayLogic.FinishAllHandsInPlay(sessId);
                    await gamePlayLogic.PayOutBetsToBettingBoxes(sessId);
                    await Task.Delay(Constants.BetweenCardsDelayMs);
                    
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
                    sessionLogic.ZeroBettingTimer(sessId);
                    await hub.SendMessageToGroupAsync(sessId, SignalRMethods.BettingReset, session.ToDto(getPlayerName));
                    
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