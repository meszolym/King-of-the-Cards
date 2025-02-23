using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KC.App.Models.Structs;

namespace KC.App.Models.Classes.Hand
{
    public abstract class Hand(List<Card> cards)
    {
        public abstract IEnumerable<Card> CardsVisible { get; }

        [JsonIgnore]
        public List<Card> Cards { get; set; } = cards;

        public bool Finished { get; set; } = false;
    }
}
