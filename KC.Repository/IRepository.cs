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
        Either<Exception, TValue> Get(TKey id);
        IEnumerable<TValue> GetAll();
        Either<Exception,TValue> Add(TValue entity);
        Either<Exception, TValue> Update(TValue entity);
        Either<Exception, TValue> Delete(TKey id);

    }
}
