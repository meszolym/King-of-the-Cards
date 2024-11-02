using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Repository
{
    public interface IRepository<TV, TK> where TV : class
    {
        Either<Exception, TV> Get(TK id);
        IEnumerable<TV> GetAll();
        Either<Exception,TV> Add(TV entity);
        Either<Exception, TV> Update(TV entity);
        Either<Exception, TV> Delete(TK id);

    }
}
