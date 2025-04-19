using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.SignalR.Client;

namespace KC.Frontend.Client.Services;

public partial class ExternalCommunicatorService
{
    public static class SignalREvents
    {
        public static IObservable<SessionReadDto> SessionCreated => SessionCreatedSubject.AsObservable();
        public static IObservable<Guid> SessionDeleted => SessionDeletedSubject.AsObservable();
    
        private static readonly Subject<SessionReadDto> SessionCreatedSubject = new Subject<SessionReadDto>();
        private static readonly Subject<Guid> SessionDeletedSubject = new Subject<Guid>();

        public static void Init(HubConnection conn)
        {
            conn.On<SessionReadDto>("SessionCreated", dto => SessionCreatedSubject.OnNext(dto));
            conn.On<Guid>("SessionDeleted", id => SessionDeletedSubject.OnNext(id));
        }
    }
}
