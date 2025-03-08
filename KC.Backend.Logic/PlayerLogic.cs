using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic;

public class PlayerLogic(IList<Player> players)
{
    public void AddPlayer(Player player) => players.Add(player);
    
    public void RemovePlayer(Guid playerId) => players.Remove(players.Single(p => p.Id == playerId));
    
    public Player Get(Guid playerId) => players.Single(p => p.Id == playerId);
    
    public void UpdateName(Guid playerId, string name) => players.Single(p => p.Id == playerId).Name = name;
    
    public void UpdateBalance(Guid playerId, int balance) => players.Single(p => p.Id == playerId).Balance = balance;
}