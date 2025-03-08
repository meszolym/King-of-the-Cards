using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerLogic
{
    void AddPlayer(Player player);
    void RemovePlayer(Guid playerId);
    Player Get(Guid playerId);
    void UpdateName(Guid playerId, string name);
    void UpdateBalance(Guid playerId, int balance);
}