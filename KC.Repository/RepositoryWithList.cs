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
    internal class RepositoryWithList<TValue, TKey> : IRepository<TValue, TKey> where TValue : class, IIdentityBearer<TKey> where TKey : IComparable
    {
        private ImmutableList<TValue> _list = ImmutableList.Create<TValue>();

        //impure
        public Fin<ImmutableList<TValue>> Add(TValue entity)
            => Get(entity.Id).Match<Fin<ImmutableList<TValue>>>(
                Succ: _ => Error.New("Item already exists"),
                Fail: _ =>
                {
                    _list = _list.Add(entity);
                    return _list;
                });
        public Fin<ImmutableList<TValue>> Delete(TKey id)
            => Get(id).Match(
                Succ: Remove,
                Fail: er => er);

        private Fin<ImmutableList<TValue>> Remove(TValue entity)
            => Try.lift(() => _list.Remove(entity)).Run();

        public Fin<TValue> Get(TKey id)
            => Try.lift(()
                    => _list.Single(item => item.Id.Equals(id))).Run()
                .Match<Fin<TValue>>(
                    Succ: item => item,
                    Fail: er => Error.New(er.Message));

        public IEnumerable<TValue> GetAll() => _list;

        //impure
        public Fin<ImmutableList<TValue>> Update(TValue entity)
            => Get(entity.Id).Match<Fin<ImmutableList<TValue>>>(
                Succ: item =>
                {
                    _list = _list.Replace(item, entity);
                    return _list;
                },
                Fail: er => er);
    }
}
