using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Logic.Extensions;
using KC.Models;
using KC.Repository;
using LanguageExt;
using LanguageExt.Common;

namespace KC.Logic.Logics
{
    public class BoxLogic(IRepository<Table, Guid> tableRepository, IRepository<Player, string> playerRepository)
    {
        public Fin<BettingBox> ClaimBox(string playerId, Guid tableId, int boxIdx) 
            => GetBoxFromTable(tableId, boxIdx)
                .Bind(b => b.Owner.Match<Fin<BettingBox>>(
                    Some: _ => Error.New("Box already claimed"),
                    None: () => playerRepository.Get(playerId).Match<Fin<BettingBox>>(
                        Succ: p => b.SetOwner(p),
                        Fail: er => er)));

        public Fin<BettingBox> UnclaimBox(string playerId, Guid tableId, int boxIdx)
            => GetBoxFromTable(tableId,boxIdx)
                .Bind(b => b.Owner.Match<Fin<BettingBox>>(
                    Some: p => p.HardwareID == playerId
                            ? b.SetOwner(Option<Player>.None) : Error.New("Box not claimed by player"),
                    None: () => Error.New("Box not claimed")));

        /// <summary>
        /// Sets the bet on the box if the box is claimed by the player. Does not handle player balance.
        /// </summary>
        public Fin<BettingBox> Bet(string playerId, Guid tableId, int boxIdx, int amount)
            => GetBoxFromTable(tableId, boxIdx)
                .Bind(b => b.Owner.Match<Fin<BettingBox>>(
                    Some: p => p.HardwareID == playerId
                        ? b.SetBet(amount) : Error.New("Box not claimed by player"),
                    None: () => Error.New("Box not claimed")));

        private Fin<BettingBox> GetBoxFromTable(Guid tableId, int boxIdx)
            => tableRepository.Get(tableId)
                .Bind(t => Try.lift(() => t.Boxes.ElementAt(boxIdx)).Run()
                    .Match<Fin<BettingBox>>(
                        Succ: b => b,
                        Fail: _ => Error.New("Box not found")));
    }
}
