using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR.Client;
using KC.Frontend.Client.Extensions;
using ReactiveUI;

namespace KC.Frontend.Client.Services;

public partial class ExternalCommunicatorService
{
    public static class SignalREvents
    {
        public static IObservable<SessionReadDto> SessionCreated => SessionCreatedSubject.AsObservable();
        public static IObservable<Guid> SessionDeleted => SessionDeletedSubject.AsObservable();
        public static IObservable<PlayerReadDto> PlayerBalanceUpdated => PlayerBalanceUpdatedSubject.AsObservable();
        public static IObservable<(Guid sessionId, int remainingSeconds)> BettingTimerTicked => BettingTimerTickedSubject.AsObservable();
        public static IObservable<Guid> BettingTimerElapsed => BettingTimerElapsedSubject.AsObservable();
        public static IObservable<Guid> BettingTimerStopped => BettingTimerStoppedSubject.AsObservable();
        public static IObservable<SessionReadDto> HandsUpdated => HandsUpdatedSubject.AsObservable();
        public static IObservable<BettingBoxReadDto> BetUpdated => BetUpdatedSubject.AsObservable();
        public static IObservable<BettingBoxReadDto> BoxOwnerChanged => BoxOwnerChangedSubject.AsObservable();
        public static IObservable<(Guid sessionId, int change)> SessionOccupancyChanged => SessionOccupancyChangedSubject.AsObservable();
    
        private static readonly Subject<SessionReadDto> SessionCreatedSubject = new();
        private static readonly Subject<Guid> SessionDeletedSubject = new();
        private static readonly Subject<PlayerReadDto> PlayerBalanceUpdatedSubject = new();
        private static readonly Subject<(Guid sessionId, int remainingSeconds)> BettingTimerTickedSubject = new();
        private static readonly Subject<Guid> BettingTimerElapsedSubject = new();
        private static readonly Subject<Guid> BettingTimerStoppedSubject = new();
        private static readonly Subject<SessionReadDto> HandsUpdatedSubject = new();
        private static readonly Subject<BettingBoxReadDto> BetUpdatedSubject = new();
        private static readonly Subject<BettingBoxReadDto> BoxOwnerChangedSubject = new();
        private static readonly Subject<(Guid sessionId, int change)> SessionOccupancyChangedSubject = new();
        
        
        public static void Init(HubConnection conn)
        {
            conn.On(SignalRMethods.SessionCreated, dto => SessionCreatedSubject.OnNext(dto));
            conn.On(SignalRMethods.SessionDeleted, id => SessionDeletedSubject.OnNext(id));
            conn.On(SignalRMethods.PlayerBalanceUpdated, dto => PlayerBalanceUpdatedSubject.OnNext(dto));
            conn.On(SignalRMethods.BettingTimerTicked, id => BettingTimerTickedSubject.OnNext(id));
            conn.On(SignalRMethods.BettingTimerElapsed, id => BettingTimerElapsedSubject.OnNext(id));
            conn.On(SignalRMethods.BettingTimerStopped, id => BettingTimerStoppedSubject.OnNext(id));
            conn.On(SignalRMethods.HandsUpdated, dto => HandsUpdatedSubject.OnNext(dto));
            conn.On(SignalRMethods.BetUpdated, dto => BetUpdatedSubject.OnNext(dto));
            conn.On(SignalRMethods.BoxOwnerChanged, dto => BoxOwnerChangedSubject.OnNext(dto));
            conn.On(SignalRMethods.SessionOccupancyChanged, dto => SessionOccupancyChangedSubject.OnNext(dto));
            
        }
    }
}