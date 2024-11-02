using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Interfaces
{
    public interface IIdentityBearer<T> where T : IComparable
    {
        T Id { get; }
    }
}
