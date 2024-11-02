using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public class Player(string hardwareId, string name, double balance)
    {
        public string HardwareID { get; set; } = hardwareId;
        public string Name { get; set; } = name;
        public double Balance { get; set; } = balance;
    }
}
