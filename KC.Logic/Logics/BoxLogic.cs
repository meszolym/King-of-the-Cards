using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using KC.Repository;
using LanguageExt;

namespace KC.Logic.Logics
{
    public class BoxLogic(IRepository<Table, Guid> tableRepository, IRepository<Player, string> playerRepository)
    {
        private IRepository<Table, Guid> _tableRepository = tableRepository;
        private IRepository<Player, string> _playerRepository = playerRepository;

        public Either<Exception, BettingBox> ClaimBox(string playerId, BettingBox box)
        {
            throw new NotImplementedException();
        }

        public Either<Exception, BettingBox> UnclaimBox(string playerId, BettingBox box)
        {
            throw new NotImplementedException();
        }

        public Either<Exception, BettingBox> Bet(string playerId, BettingBox box, int amount)
        {
            throw new NotImplementedException();
        }
    }
}
