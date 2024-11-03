using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using KC.Repository;
using LanguageExt;
using LanguageExt.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = LanguageExt.Common.Error;

namespace KC.Logic.Logics
{
    public class BoxLogic(IRepository<BettingBox, (Guid,int)> boxRepository, IRepository<Player,string> playerRepository)
    {
        public Fin<BettingBox> Get((Guid, int) key) =>
            boxRepository.Get(key);

        public Fin<BettingBox> ClaimBox(string playerId, Guid tableId, int boxIdx) 
            => Get((tableId,boxIdx))
                .Bind(b => b.Owner.Match<Fin<BettingBox>>(
                    Some: _ => Error.New("Box already claimed"),
                    None: () => playerRepository.Get(playerId).Match<Fin<BettingBox>>(
                        Succ: p => boxRepository.Update(b.GetBuilder().SetOwner(p.Id).Build()).Bind(boxes => Get(b.Id)),
                        Fail: er => er)));

        public Fin<BettingBox> UnclaimBox(string playerId, Guid tableId, int boxIdx)
            => Get((tableId, boxIdx))
                .Bind(b => b.Owner.Match<Fin<BettingBox>>(
                    Some: p => p.Id.Equals(playerId) 
                        ? boxRepository.Update(b.GetBuilder().SetOwner(Option<string>.None).Build()).Bind(boxes => Get(b.Id)) 
                        : Error.New("Box not owned by player"),
                    None: () => Error.New("Box not claimed")));

        /// <summary>
        /// Sets the bet on the box if the box is claimed by the player. Does not handle player balance.
        /// </summary>
        public Fin<BettingBox> Bet(string playerId, Guid tableId, int boxIdx, int amount)
            => Get((tableId, boxIdx))
                .Bind(b => b.Owner.Match<Fin<BettingBox>>(
                    Some: p => p.Id.Equals(playerId)
                        ? boxRepository.Update(/*!!!!HANDS[0].BET = Amount!!!!*/).Bind(boxes => Get(b.Id))
                        : Error.New("Box not owned by player"),
                    None: () => Error.New("Box not claimed")));

    }
}
