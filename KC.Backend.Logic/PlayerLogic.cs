using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic
{
    public class PlayerLogic(IList<Player> dataStore) : IPlayerLogic
    {
        public IEnumerable<Player> GetAll() => dataStore;

        public void Add(Player item) => dataStore.Add(item);

        public void Remove(Guid id) => dataStore.Remove(Get(id));

        public Player Get(Guid id) => dataStore.Single(p => p.Id == id);

        public void Update(Player item)
        {
            var internalPlayer = Get(item.Id);
            internalPlayer.Name = item.Name;
            internalPlayer.ConnectionId = item.ConnectionId;
            internalPlayer.Balance = item.Balance;
        }
    }
}
