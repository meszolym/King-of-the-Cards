using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics;

public class PlayerLogic(IList<Player> players, IDictionary<MacAddress, Guid> macToPlayerGuid) : IPlayerLogic
{
    public void AddPlayer(MacAddress mac, Player player, double balance = 500)
    {
        var p = player.WithBalance(balance);
        players.Add(p);
        macToPlayerGuid.Add(mac, p.Id);
    }
    
    public void RemovePlayer(MacAddress playerId) => players.Remove(players.Single(p => p.Id == macToPlayerGuid[playerId]));
    
    public Player Get(MacAddress playerId) => Get(macToPlayerGuid[playerId]);

    public Player Get(Guid playerId) => playerId != Guid.Empty ? players.Single(p => p.Id == playerId) : Player.None;
    
    public void UpdateName(MacAddress playerId, string name) => players.Single(p => p.Id == macToPlayerGuid[playerId]).Name = name;

    public void AddToBalance(MacAddress playerId, double amount) => AddToBalance(macToPlayerGuid[playerId], amount);
    
    public void AddToBalance(Guid playerId, double amount) => UpdateBalance(playerId, Get(playerId).Balance + amount);

    public void UpdateBalance(MacAddress playerId, double balance) => UpdateBalance(macToPlayerGuid[playerId], balance);

    public void UpdateBalance(Guid playerId, double balance)
    {
        if (balance < 0)
            throw new ArgumentException("Player balance can not be negative.");
        
        players.Single(p => p.Id == playerId).Balance = balance;
    }

    public void UpdatePlayerConnectionId(MacAddress playerId, string connectionId) => players.Single(p => p.Id == macToPlayerGuid[playerId]).ConnectionId = connectionId;
}