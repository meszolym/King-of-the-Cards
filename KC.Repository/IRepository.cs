using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Repository
{
    public interface IRepository<TValue, TKey> where TValue : class
    {
        Fin<TValue> Get(TKey id);
        IEnumerable<TValue> GetAll();
        Fin<ImmutableList<TValue>> Add(TValue entity);
        Fin<ImmutableList<TValue>> Update(TValue entity);
        Fin<ImmutableList<TValue>> Delete(TKey id);

    }
}
