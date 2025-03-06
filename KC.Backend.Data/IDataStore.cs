using KC.App.Backend.Models.Interfaces;

namespace KC.Backend.Data;

public interface IDataStore<TVal, TKey> where TVal : class, IIdentityBearer<TKey> where TKey : IComparable
{
    IEnumerable<TVal> GetAll();
    void Add(TVal item);
    void Remove(TKey id);
    TVal Get(TKey id);

    //There's no update, as the data store is supposed to be a simple in-memory data store
}