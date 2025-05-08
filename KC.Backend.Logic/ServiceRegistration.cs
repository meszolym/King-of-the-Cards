using System;
using KC.Backend.Logic.Core;
using KC.Backend.Logic.Core.Interfaces;
using KC.Backend.Logic.Logics;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services;
using KC.Backend.Logic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Backend.Logic;

public delegate string GetPlayerNameDelegate(Guid playerId);

public static class ServiceRegistration
{
    public static IServiceCollection RegisterLogicLayerServices(this IServiceCollection services)
    {
        services.AddTransient<IBettingBoxLogic, BettingBoxLogic>();
        services.AddTransient<IGamePlayLogic, GamePlayLogic>();
        services.AddTransient<IPlayerLogic, PlayerLogic>();

        services.AddSingleton<GetPlayerNameDelegate>(s =>
        {
            var logic = s.GetRequiredService<IPlayerLogic>();
            return (Guid playerId) => logic.Get(playerId).Name;
        });
            
        services.AddTransient<ISessionLogic, SessionLogic>();
        services.AddTransient<ISessionTerminatorService, SessionTerminatorService>();
            
        //core
        services.AddTransient<IRuleBook, RuleBook>();
        
        return services;
    }
}