using KC.Backend.API.Extensions;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic;
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
    
    public static IServiceCollection RegisterEventDelegates(this IServiceCollection services)
    {
        //OnTurnInfoChanged
        services.AddSingleton<OnTurnInfoChangedDelegate>(s =>
        {
            var sessionLogic = s.GetRequiredService<ISessionLogic>();
            var hub = s.GetRequiredService<IClientCommunicator>();
            var gamePlayLogic = s.GetRequiredService<IGamePlayLogic>();
            var getPlayerName = s.GetRequiredService<GetPlayerNameDelegate>();

            return async sessId =>
            {
                var session = sessionLogic.Get(sessId);
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.TurnChanged, session.CurrentTurnInfo);
                if (session.CurrentTurnInfo.PlayersTurn) return;

                await gamePlayLogic.DealerPlayHand(sessId, async () =>
                {
                    await hub.SendMessageToGroupAsync(sessId, SignalRMethods.HandsUpdated,
                        session.ToDto(getPlayerName));
                    await Task.Delay(TimeSpan.FromSeconds(DelaySecsBetweenCards));
                });
                
                //TODO: Check winners, pay out bets, clear hands
                
            };
        });
        
        //OnBettingTimerTicked
        services.AddSingleton<OnBettingTimerTickedDelegate>(s =>
        {
            var hub = s.GetRequiredService<ClientCommunicator>();
            return async (Guid sessionId, int remainingSeconds) => await hub.SendMessageToGroupAsync(sessionId,
                SignalRMethods.BettingTimerTicked, (sessionId, remainingSeconds));
        });
        
        //OnBettingTimerElapsed
        services.AddSingleton<OnBettingTimerElapsedDelegate>(s =>
        {
            var hub = s.GetRequiredService<ClientCommunicator>();
            var gamePlayLogic = s.GetRequiredService<IGamePlayLogic>();
            var sessionLogic = s.GetRequiredService<ISessionLogic>();
            var getPlayerName = s.GetRequiredService<GetPlayerNameDelegate>();

            return async sessId =>
            {
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.BettingTimerElapsed, sessId);

                gamePlayLogic.ShuffleIfNeeded(sessId);
        
                var session = sessionLogic.Get(sessId);
                await gamePlayLogic.DealStartingCards(sessId, async () =>
                {
                    await hub.SendMessageToGroupAsync(sessId, SignalRMethods.HandsUpdated,
                        session.ToDto(getPlayerName));
                    await Task.Delay(TimeSpan.FromSeconds(DelaySecsBetweenCards));
                });
                gamePlayLogic.TransferTurn(sessId);
                await hub.SendMessageToGroupAsync(sessId, SignalRMethods.TurnChanged, session.CurrentTurnInfo);
            };
        });

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
        
        return services;
    }
}