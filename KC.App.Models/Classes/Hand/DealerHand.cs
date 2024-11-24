using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.App.Models.Structs;
using Newtonsoft.Json;

namespace KC.App.Models.Classes.Hand
{
    public class DealerHand(List<Card> cards) : Hand(cards)
    {
        [JsonIgnore] 
        public bool ShowsCards = false;

        public override IEnumerable<Card> CardsVisible
        {
            get => ShowsCards ? Cards : Cards.Take(1);
        }
    }
}
