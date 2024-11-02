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
    public class PlayerLogic(IRepository<Player, string> repository)
    {
        public Fin<Player> Get(string id) => repository.Get(id);

        public IEnumerable<Player> GetAll() => repository.GetAll();

        public Fin<IEnumerable<Player>> Add(Player entity) => repository.Add(entity).Match<Fin<IEnumerable<Player>>>(
            Succ: imm => imm.ToList(),
            Fail: er => er
            );

        public Fin<IEnumerable<Player>> Update(Player entity) => repository.Update(entity).Match<Fin<IEnumerable<Player>>>(
            Succ: imm => imm.ToList(),
            Fail: er => er
        );

        public Fin<IEnumerable<Player>> Delete(string id) => repository.Delete(id).Match<Fin<IEnumerable<Player>>>(
            Succ: imm => imm.ToList(),
            Fail: er => er
        );
    }
}
