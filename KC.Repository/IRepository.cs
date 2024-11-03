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
        Fin<IEnumerable<TValue>> Add(TValue entity);
        Fin<IEnumerable<TValue>> Update(TValue entity);
        Fin<IEnumerable<TValue>> Delete(TKey id);

    }
}
