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

namespace KC.Logic.Logics
{
    public class BoxLogic(IRepository<Table, Guid> tableRepository, IRepository<Player, string> playerRepository)
    {
        public Either<Exception, BettingBox> ClaimBox(string playerId, Guid tableId, int boxIdx) 
            => GetBoxFromTable(tableId, boxIdx)
                .Bind(b => b.Owner.Match<Either<Exception,BettingBox>>(
                    Some: _ => new Exception("Box already claimed"),
                    None: () => playerRepository.Get(playerId).Match<Either<Exception,BettingBox>>(
                        Right: p => b.SetOwner(p),
                        Left: ex => ex)));

        public Either<Exception, BettingBox> UnclaimBox(string playerId, Guid tableId, int boxIdx)
            => GetBoxFromTable(tableId,boxIdx)
                .Bind(b => b.Owner.Match<Either<Exception, BettingBox>>(
                    Some: p => p.HardwareID == playerId
                            ? b.SetOwner(Option<Player>.None) : new Exception("Box not claimed by player"),
                    None: () => new Exception("Box not claimed")));

        public Either<Exception, BettingBox> Bet(string playerId, Guid tableId, int boxIdx, int amount)
            => GetBoxFromTable(tableId, boxIdx)
                .Bind(b => b.Owner.Match<Either<Exception, BettingBox>>(
                    Some: p => p.HardwareID == playerId
                        ? b.SetBet(amount) : new Exception("Box not claimed by player"),
                    None: () => new Exception("Box not claimed")));

        private Either<Exception, BettingBox> GetBoxFromTable(Guid tableId, int boxIdx)
            => tableRepository.Get(tableId)
                .Bind(t => Try.lift(() => t.Boxes.ElementAt(boxIdx)).Run()
                    .Match<Either<Exception, BettingBox>>(
                        Succ: b => b,
                        Fail: _ => new Exception("Box not found")));
    }
}
