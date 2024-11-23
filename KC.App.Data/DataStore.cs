using KC.App.Models.Interfaces;

namespace KC.App.Data
{
    public class DataStore<TVal, TKey>(List<TVal> data) : IDataStore<TVal, TKey> where TVal : class, IIdentityBearer<TKey> where TKey : IComparable
    {
        public IEnumerable<TVal> GetAll() => data;

        public void Add(TVal item)
        {
            if (Get(item.Id) is not null)
            {
                throw new ArgumentException("Item with this Id already exists");
            }

            data.Add(item);

            return;
        }

        public void Remove(TKey id)
        {
            var item = Get(id);
            if (item is null)
            {
                throw new ArgumentException("Item with this Id does not exist");
            }

            data.Remove(item);
        }

        public TVal? Get(TKey id) => data.SingleOrDefault(d => d.Id.Equals(id));

        public void Update(TVal item)
        {
            if (Get(item.Id) is null)
            {
                throw new ArgumentException("Item with this Id does not exist");
            }

            data[data.FindIndex(d => d.Id.Equals(item.Id))] = item;
        }
    }
}
