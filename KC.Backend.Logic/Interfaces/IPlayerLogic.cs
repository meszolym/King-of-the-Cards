using KC.Backend.Models;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerLogic
{
    IEnumerable<Player> GetAll();
    void Add(Player item);
    void Remove(Guid id);
    Player Get(Guid id);
    void Update(Player item);
}