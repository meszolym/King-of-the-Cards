using System.Net.NetworkInformation;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic;

//This is done.
public class PlayerLogic(IList<Player> players) : IPlayerLogic
{
    public void AddPlayer(Player player) => players.Add(player);
    
    public void RemovePlayer(MacAddress playerId) => players.Remove(players.Single(p => p.Id == playerId));
    
    public Player Get(MacAddress playerId) => players.Single(p => p.Id == playerId);
    
    public void UpdateName(MacAddress playerId, string name) => players.Single(p => p.Id == playerId).Name = name;
    
    public void UpdateBalance(MacAddress playerId, int balance) => players.Single(p => p.Id == playerId).Balance = balance;
}