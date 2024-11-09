using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Interfaces
{
    public interface IIdentityBearer<T>
    {
        T Id { get; }
    }
}
