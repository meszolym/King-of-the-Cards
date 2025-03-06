using KC.App.Backend.Models.Classes;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerLogic
{
    IEnumerable<Player> GetAll();
    void Add(Player item);
    void Remove(string id);
    Player Get(string id);
    void Update(Player item);
}