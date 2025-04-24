using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using ReactiveUI;

namespace KC.Frontend.Client.Services;

public partial class ExternalCommunicatorService
{
    public static class SignalREvents
    {
        public static IObservable<SessionReadDto> SessionCreated => SessionCreatedSubject.AsObservable();
        public static IObservable<Guid> SessionDeleted => SessionDeletedSubject.AsObservable();
        public static IObservable<PlayerReadDto> PlayerBalanceUpdated => PlayerBalanceUpdatedSubject.AsObservable();
        public static IObservable<Guid> BettingTimerTicked => BettingTimerTickedSubject.AsObservable();
        public static IObservable<Guid> BettingTimerElapsed => BettingTimerElapsedSubject.AsObservable();
    
        private static readonly Subject<SessionReadDto> SessionCreatedSubject = new Subject<SessionReadDto>();
        private static readonly Subject<Guid> SessionDeletedSubject = new Subject<Guid>();
        private static readonly Subject<PlayerReadDto> PlayerBalanceUpdatedSubject = new Subject<PlayerReadDto>();
        private static readonly Subject<Guid> BettingTimerTickedSubject = new Subject<Guid>();
        private static readonly Subject<Guid> BettingTimerElapsedSubject = new Subject<Guid>();
        
        public static void Init(HubConnection conn)
        {
            conn.On<SessionReadDto>("SessionCreated", dto => SessionCreatedSubject.OnNext(dto));
            conn.On<Guid>("SessionDeleted", id => SessionDeletedSubject.OnNext(id));
            conn.On<PlayerReadDto>("PlayerBalanceUpdated", dto => PlayerBalanceUpdatedSubject.OnNext(dto));
            conn.On<Guid>("BettingTimerTicked", id => BettingTimerTickedSubject.OnNext(id));
            conn.On<Guid>("BettingTimerElapsed", id => BettingTimerElapsedSubject.OnNext(id));
        }
    }
}
