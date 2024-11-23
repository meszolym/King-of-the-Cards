using KC.App.Models.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace KC.App.Data
{
    public class DataStore<TVal, TKey>(List<TVal> data) : IDataStore<TVal, TKey> where TVal : class, IIdentityBearer<TKey> where TKey : IComparable
    {
        public IEnumerable<TVal> GetAll() => data;

        public Fin<Unit> Add(TVal item) => Get(item.Id).Match<Fin<Unit>>(
            Some: _ => FinFail<Unit>(Error.New("Item with this Id already exists")),
            None: () =>
            {
                data.Add(item);
                return Unit.Default;
            }
        );

        public Fin<Unit> Remove(TKey id) => Get(id).Match<Fin<Unit>>(
            Some: _ =>
            {
                data.RemoveAll(item => item.Id.Equals(id));
                return Unit.Default;
            },
            None: () => FinFail<Unit>(Error.New("Item with this Id does not exist"))
        );

        public Option<TVal> Get(TKey id) => data.SingleOrDefault(d => d.Id.Equals(id));

        public Fin<Unit> Update(TVal item) => Get(item.Id).Match<Fin<Unit>>(
            Some: _ =>
            {
                data[data.FindIndex(d => d.Id.Equals(item.Id))] = item;
                return Unit.Default;
            },
            None: () => FinFail<Unit>(Error.New("Item with this Id does not exist"))
        );
    }
}
