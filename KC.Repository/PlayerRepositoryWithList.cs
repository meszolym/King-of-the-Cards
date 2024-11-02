using KC.Models;
using LanguageExt;
using LanguageExt.Common;

namespace KC.Repository
{
    public class PlayerRepositoryWithList : IRepository<Player,string>
    {
        private readonly List<Player> _players = [];

        public Fin<Player> Add(Player entity)
            => Get(entity.HardwareID).Match<Fin<Player>>(
                Succ: _ => Error.New("Player already exists"),
                Fail: _ =>
                {
                    _players.Add(entity);
                    return entity;
                });
        
        public Fin<Player> Delete(string id) 
            => Get(id).Match<Fin<Player>>(
                Succ: Remove, 
                Fail: er => er);

        private Fin<Player> Remove(Player player)
            => Try.lift(() => _players.Remove(player)).Run()
                .Match<Fin<Player>>(
                    Succ: _ => player,
                    Fail: er => er);

        public Fin<Player> Get(string id) 
            => Try.lift(() 
                    => _players.Single(p => p.HardwareID == id)).Run()
                .Match<Fin<Player>>(
                    Succ: p => p,
                    Fail: er => er);

        public IEnumerable<Player> GetAll() => _players;

        public Fin<Player> Update(Player entity)
            => Get(entity.HardwareID).Match<Fin<Player>>(
                Succ: p =>
                {
                    var index = _players.IndexOf(p);
                    _players[index] = entity;
                    return p;
                },
                Fail: er => er);
    }
}
