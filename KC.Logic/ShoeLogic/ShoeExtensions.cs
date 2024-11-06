using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Classes;

namespace KC.Logic.ShoeLogic
{
    public static class ShoeExtensions
    {
        public static Shoe Shuffle(this Shoe shoe, Random random)
        {
            for (int i = 0; i < shoe.Cards.Count; i++)
            {
                int j = random.Next(i, shoe.Cards.Count);
                (shoe.Cards[i], shoe.Cards[j]) = (shoe.Cards[j], shoe.Cards[i]);
            }

            return shoe;
        }
    }
}
