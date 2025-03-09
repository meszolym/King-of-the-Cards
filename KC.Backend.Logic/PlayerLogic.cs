using System.Net.NetworkInformation;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic;

//This is done.
public class PlayerLogic(IList<Player> players) : IPlayerLogic
{
    public void AddPlayer(Player player) => players.Add(player);
    
    public void RemovePlayer(PhysicalAddress playerId) => players.Remove(players.Single(p => p.Id == playerId));
    
    public Player Get(PhysicalAddress playerId) => players.Single(p => p.Id == playerId);
    
    public void UpdateName(PhysicalAddress playerId, string name) => players.Single(p => p.Id == playerId).Name = name;
    
    public void UpdateBalance(PhysicalAddress playerId, int balance) => players.Single(p => p.Id == playerId).Balance = balance;
}