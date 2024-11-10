using KC.Data;
using KC.Models.Classes;
using LanguageExt;

namespace KC.Logic.PlayerLogic
{
    public class PlayerLogic(IDataStore<Player,string> dataStore)
    {
        public IEnumerable<Player> GetAll() => dataStore.GetAll();

        public Fin<Unit> Add(Player item) => dataStore.Add(item);

        public Fin<Unit> Remove(string id) => dataStore.Remove(id);

        public Fin<Player> Get(string id) => dataStore.Get(id);

        public Fin<Unit> Update(Player item) => dataStore.Update(item);
    }
}
