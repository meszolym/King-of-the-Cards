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
    internal class Repository<TValue, TKey>(KcDbContext DbContext) : IRepository<TValue, TKey> where TValue : class, IIdentityBearer<TKey> where TKey : IComparable
    {
        public Fin<IEnumerable<TValue>> Add(TValue entity)
            => Get(entity.Id).Match<Fin<IEnumerable<TValue>>>(
                Succ: _ => Error.New("Item already exists"),
                Fail: _ =>
                {
                    DbContext.Set<TValue>().Add(entity);
                    DbContext.SaveChanges();
                    return (Fin<IEnumerable<TValue>>)GetAll();
                });
        public Fin<IEnumerable<TValue>> Delete(TKey id)
            => Get(id).Match(
                Succ: Remove,
                Fail: er => er);

        private Fin<IEnumerable<TValue>> Remove(TValue entity)
            => Try.lift(() =>
            {
                DbContext.Set<TValue>().Remove(entity);
                DbContext.SaveChanges();
                return (Fin<IEnumerable<TValue>>)GetAll();
            }).Run();

        public Fin<TValue> Get(TKey id)
            => Try.lift(()
                    => DbContext.Set<TValue>().Single(item => item.Id.Equals(id))).Run()
                .Match<Fin<TValue>>(
                    Succ: item => item,
                    Fail: er => Error.New(er.Message));

        public IEnumerable<TValue> GetAll() => DbContext.Set<TValue>();

        public Fin<IEnumerable<TValue>> Update(TValue entity)
            => Get(entity.Id).Match<Fin<IEnumerable<TValue>>>(
                Succ: item =>
                {
                    DbContext.Update(entity);
                    DbContext.SaveChanges();
                    return (Fin<IEnumerable<TValue>>)GetAll();
                },
                Fail: er => er);
    }
}
