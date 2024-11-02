using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record Player(string HardwareId, string Name, double Balance)
    {
        public string HardwareId { get; } = HardwareId;
        public string Name { get; } = Name;
        public double Balance { get; } = Balance;
    }
}
