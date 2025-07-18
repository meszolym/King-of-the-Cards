﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Services;
using KC.Frontend.Client.Utilities;
using KC.Frontend.Client.ViewModels.Components;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using Splat;
namespace KC.Frontend.Client.ViewModels
{
    partial class MenuViewModel : ReactiveObject, IRoutableViewModel
    {
        private IObservable<bool> _joinCanExecute;
        private readonly ExternalCommunicatorService _externalCommunicator;
        
        [Reactive]
        private bool _sessionGetErrored = true;
        
        [Reactive]
        private bool _noSessions;
        
        public MenuViewModel(IScreen host)
        {
            HostScreen = host;
            _joinCanExecute = this.WhenAnyValue(x => x.SelectedItemViewModel).Select(x => x != null!);
            
            _externalCommunicator = Locator.Current.GetRequiredService<ExternalCommunicatorService>();

            JoinSessionCommand.ThrownExceptions.Subscribe(e => Debug.WriteLine(e.Message));
            
            ExternalCommunicatorService.SignalREvents.SessionCreated.ObserveOn(RxApp.MainThreadScheduler).Subscribe(dto => Sessions.Add(dto.ToSessionListItem()));
            ExternalCommunicatorService.SignalREvents.SessionDeleted.ObserveOn(RxApp.MainThreadScheduler).Subscribe(g =>
            {
                Sessions.Remove(Sessions.FirstOrDefault(x => x.Id == g)!);
                
                if (HostScreen.Router.GetCurrentViewModel() is SessionViewModel)
                    HostScreen.Router.NavigateBack.Execute();
            });
            ExternalCommunicatorService.SignalREvents.SessionOccupancyChanged.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(dto =>
                {
                    var s = Sessions.First(s => s.Id == dto.Id);
                    s.CurrentOccupancy = dto.Table.BettingBoxes.Count(b => b.OwnerId != Guid.Empty);
                    s.TooltipText = dto.Table.BettingBoxes.Count(b => b.OwnerId != Guid.Empty) == 0
                        ? "Players:" + Environment.NewLine + "None"
                        : "Players:" + Environment.NewLine + string.Join(Environment.NewLine,
                            dto.Table.BettingBoxes.Select(b => b.OwnerName).Distinct());
                });
            
            Sessions.CollectionChanged += (_, __) =>
                this.RaisePropertyChanged(nameof(Sessions.Count));

            this.WhenAnyValue(x => x.Sessions.Count)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(count => count == 0)
                .BindTo(this, x => x.NoSessions);

            CreateSessionCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"{ex.Message} in {nameof(CreateSessionCommand)}"));

        }

        [Reactive]
        //Can't be null, but compiler gives warning :)
        private SessionListItemViewModel _selectedItemViewModel = null!;

        [ReactiveCommand(CanExecute = nameof(_joinCanExecute))]
        private async Task JoinSession()
        {
            Debug.WriteLine("Joining session");
            await _externalCommunicator.JoinSession(SelectedItemViewModel.Id, ClientMacAddressHandler.PrimaryMacAddress);
            var session = await _externalCommunicator.GetSession(SelectedItemViewModel.Id);
            await HostScreen.Router.Navigate.Execute(new SessionViewModel(HostScreen, session));
        }

        [ReactiveCommand]
        private async Task CreateSession() => await _externalCommunicator.CreateSession();

        [ReactiveCommand]
        private async Task TryGetSessions()
        {
            try
            {
                Sessions = [..(await _externalCommunicator.GetSessionList())];
                SessionGetErrored = false;
            }
            catch (Exception e)
            {
                SessionGetErrored = true;
            }
        }

        // [Reactive]
        // private List<SessionListItem> _sessions = new List<SessionListItem>();
        
        [Reactive]
        private ObservableCollection<SessionListItemViewModel> _sessions = [];

        public string? UrlPathSegment { get; } = "menu";

        public IScreen HostScreen { get; }
    }
}
