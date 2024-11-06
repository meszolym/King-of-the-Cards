using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Common;
using KC.Models.Interfaces;

namespace KC.Repository
{
    internal class RepositoryWithSet<TValue, TKey> : IRepository<TValue, TKey> where TValue : class, IIdentityBearer<TKey> where TKey : IComparable
    {
        ImmutableDictionary<TKey, TValue> _items = ImmutableDictionary<TKey, TValue>.Empty;
        public Fin<IEnumerable<TValue>> Add(TValue entity)
            => Get(entity.Id).Match<Fin<IEnumerable<TValue>>>(
                Succ: _ => Error.New("Item already exists"),
                Fail: _ =>
                {
                    _items = _items.Add(entity.Id, entity);
                    return (Fin<IEnumerable<TValue>>)_items.Values;
                });
        public Fin<IEnumerable<TValue>> Delete(TKey id)
            => Get(id).Match(
                Succ: Remove,
                Fail: er => er);

        private Fin<IEnumerable<TValue>> Remove(TValue entity)
            => Try.lift(() =>
            {
                _items = _items.Remove(entity.Id);
                return (Fin<IEnumerable<TValue>>)_items.Values;
            }).Run();

        public Fin<TValue> Get(TKey id)
            => Try.lift(()
                    => _items[id]).Run()
                .Match<Fin<TValue>>(
                    Succ: item => item,
                    Fail: er => Error.New(er.Message));

        public IEnumerable<TValue> GetAll() => _items.Values;

        public Fin<IEnumerable<TValue>> Update(TValue entity)
            => Get(entity.Id).Match<Fin<IEnumerable<TValue>>>(
                Succ: item =>
                {
                    _items = _items.Remove(entity.Id);
                    _items = _items.Add(entity.Id, entity);
                    return (Fin<IEnumerable<TValue>>)_items.Values;
                },
                Fail: er => er);
    }
}
