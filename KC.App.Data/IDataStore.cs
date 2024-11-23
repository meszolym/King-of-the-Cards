using KC.App.Models.Interfaces;

namespace KC.App.Data;

public interface IDataStore<TVal, TKey> where TVal : class, IIdentityBearer<TKey> where TKey : IComparable
{
    IEnumerable<TVal> GetAll();
    void Add(TVal item);
    void Remove(TKey id);
    TVal Get(TKey id);
    void Update(TVal item);
}