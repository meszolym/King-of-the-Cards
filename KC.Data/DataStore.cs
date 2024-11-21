using KC.App.Models.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace KC.App.Data
{
    public class DataStore<TVal, TKey> : IDataStore<TVal, TKey> where TVal : class, IIdentityBearer<TKey> where TKey : IComparable
    {
        private readonly List<TVal> _dataList = [];
        public IEnumerable<TVal> GetAll() => _dataList;

        public Fin<Unit> Add(TVal item) => Get(item.Id).Match<Fin<Unit>>(
            Succ: _ => FinFail<Unit>(Error.New("Item with this Id already exists")),
            Fail: _ =>
            {
                _dataList.Add(item);
                return Unit.Default;
            }
        );

        public Fin<Unit> Remove(TKey id) => Get(id).Match<Fin<Unit>>(
            Succ: _ =>
            {
                _dataList.RemoveAll(item => item.Id.Equals(id));
                return Unit.Default;
            },
            Fail: _ => FinFail<Unit>(Error.New("Item with this Id does not exist"))
        );

        public Fin<TVal> Get(TKey id) => _dataList.SingleOrDefault(d => d.Id.Equals(id)) ?? FinFail<TVal>(Error.New("Item with this Id does not exist"));

        public Fin<Unit> Update(TVal item) => Get(item.Id).Match<Fin<Unit>>(
            Succ: _ =>
            {
                _dataList[_dataList.FindIndex(d => d.Id.Equals(item.Id))] = item;
                return Unit.Default;
            },
            Fail: _ => FinFail<Unit>(Error.New("Item with this Id does not exist"))
        );
    }
}
