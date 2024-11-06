using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Interfaces;

namespace KC.Models
{
    public record Player(string Id, string Name, double Balance) : IIdentityBearer<string>
    {
        public void Deconstruct(out string Id, out string Name, out double Balance)
        {
            Id = this.Id;
            Name = this.Name;
            Balance = this.Balance;
        }
    }
}
