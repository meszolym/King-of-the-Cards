using KC.App.Backend.Models.Classes;
using KC.App.Data;
using KC.App.Logic;
using KC.App.Logic.Interfaces;

namespace KC.App.Logic.PlayerLogic
{
    public class PlayerLogic(IDataStore<Player,string> dataStore) : IPlayerLogic
    {
        public IEnumerable<Player> GetAll() => dataStore.GetAll();

        public void Add(Player item) => dataStore.Add(item);

        public void Remove(string id) => dataStore.Remove(id);

        public Player Get(string id) => dataStore.Get(id);

        public void Update(Player item)
        {
            var internalPlayer = Get(item.Id);
            internalPlayer.Name = item.Name;
            internalPlayer.ConnectionId = item.ConnectionId;
        }
    }
}
