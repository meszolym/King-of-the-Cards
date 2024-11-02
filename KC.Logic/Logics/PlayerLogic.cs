using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using KC.Repository;
using LanguageExt;

namespace KC.Logic.Logics
{
    public class PlayerLogic(IRepository<Player, string> repository)
    {
        private readonly IRepository<Player,string> _repository = repository;

        public Either<Exception, Player> Get(string id) => _repository.Get(id);

        public IEnumerable<Player> GetAll() => _repository.GetAll();

        public Either<Exception, Player> Add(Player entity) => _repository.Add(entity);

        public Either<Exception, Player> Update(Player entity) => _repository.Update(entity);

        public Either<Exception, Player> Delete(string id) => _repository.Delete(id);
    }
}
