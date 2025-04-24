using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics;

//This is done.
public class PlayerLogic(IList<Player> players, IDictionary<MacAddress, Guid> macToPlayerGuid) : IPlayerLogic
{
    private static double DefaultBalance { get; set; } = 500;

    public void AddPlayer(MacAddress mac, Player player)
    {
        var p = player.WithBalance(DefaultBalance);
        players.Add(p);
        macToPlayerGuid.Add(mac, p.Id);
    }
    
    public void RemovePlayer(MacAddress playerId) => players.Remove(players.Single(p => p.Id == macToPlayerGuid[playerId]));
    
    public Player Get(MacAddress playerId) => Get(macToPlayerGuid[playerId]);

    public Player Get(Guid playerId) => playerId != Guid.Empty ? players.Single(p => p.Id == playerId) : Player.None;
    
    public void UpdateName(MacAddress playerId, string name) => players.Single(p => p.Id == macToPlayerGuid[playerId]).Name = name;

    public void UpdateBalance(MacAddress playerId, double balance) => UpdateBalance(macToPlayerGuid[playerId], balance);

    public void UpdateBalance(Guid playerId, double balance)
    {
        if (balance < 0)
            throw new ArgumentException("Player balance can not be negative.");
        
        players.Single(p => p.Id == playerId).Balance = balance;
    }

    public void UpdatePlayerConnectionId(MacAddress playerId, string connectionId) => players.Single(p => p.Id == macToPlayerGuid[playerId]).ConnectionId = connectionId;
}