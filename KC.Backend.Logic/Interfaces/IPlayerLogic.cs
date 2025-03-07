using KC.Backend.Models;
using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerLogic
{
    IEnumerable<Player> GetAll();
    void Add(Player item);
    void Remove(Guid id);
    Player Get(Guid id);
    void Update(Player item);
}