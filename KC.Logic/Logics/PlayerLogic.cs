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
        public Fin<Player> Get(string id) => repository.Get(id);

        public IEnumerable<Player> GetAll() => repository.GetAll();

        public Fin<Player> Add(Player entity) => repository.Add(entity);

        public Fin<Player> Update(Player entity) => repository.Update(entity);

        public Fin<Player> Delete(string id) => repository.Delete(id);
    }
}
