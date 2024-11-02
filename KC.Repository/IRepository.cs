using System;
using System.Collections.Generic;
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
        Fin<TValue> Add(TValue entity);
        Fin<TValue> Update(TValue entity);
        Fin<TValue> Delete(TKey id);

    }
}
