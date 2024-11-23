using KC.App.Models.Classes;

namespace KC.App.Logic.Interfaces;

public interface IPlayerLogic
{
    IEnumerable<Player> GetAll();
    void Add(Player item);
    void Remove(string id);
    Player Get(string id);
    void Update(Player item);
}