using KC.App.Models.Classes;
using LanguageExt;

namespace KC.App.Logic.Interfaces;

public interface IPlayerLogic
{
    IEnumerable<Player> GetAll();
    Fin<Unit> Add(Player item);
    Fin<Unit> Remove(string id);
    Option<Player> Get(string id);
    Fin<Unit> Update(Player item);
}