using KC.App.Models.Interfaces;

namespace KC.App.Data
{
    public class DataStore<TVal, TKey>(List<TVal> data) : IDataStore<TVal, TKey> where TVal : class, IIdentityBearer<TKey> where TKey : IComparable
    {
        public IEnumerable<TVal> GetAll() => data;

        public void Add(TVal item)
        {
            try
            {
                Get(item.Id);
            }
            catch (Exception e)
            {
                data.Add(item);
                return;
            }
            
            throw new ArgumentException("Item with this ID already exists");
        }

        public void Remove(TKey id)
        {
            var item = Get(id);
            data.Remove(item);
        }

        public TVal Get(TKey id) => data.Single(d => d.Id.Equals(id));

        public void Update(TVal item)
        {
            data[data.IndexOf(Get(item.Id))] = item;
        }
    }
}
