using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic;

//This is done.
public class PlayerLogic(IList<Player> players) : IPlayerLogic
{
    public static double DefaultBalance { get; set; } = 500;
    public void AddPlayer(Player player) => players.Add(player.WithBalance(DefaultBalance));
    
    public void RemovePlayer(MacAddress playerId) => players.Remove(players.Single(p => p.Mac == playerId));
    
    public Player Get(MacAddress playerId) => players.Single(p => p.Mac == playerId);
    
    public void UpdateName(MacAddress playerId, string name) => players.Single(p => p.Mac == playerId).Name = name;
    
    public void UpdateBalance(MacAddress playerId, int balance) => players.Single(p => p.Mac == playerId).Balance = balance;
    
    public void UpdatePlayerConnectionId(MacAddress playerId, string connectionId) => players.Single(p => p.Mac == playerId).ConnectionId = connectionId;
}