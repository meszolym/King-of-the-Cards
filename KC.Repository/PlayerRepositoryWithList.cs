using KC.Models;
using LanguageExt;
namespace KC.Repository
{
    public class PlayerRepositoryWithList : IRepository<Player,string>
    {
        private readonly List<Player> _players = [];

        public Either<Exception, Player> Add(Player entity)
            => Get(entity.HardwareID).Match<Either<Exception, Player>>(
                Right: _ => new Exception("Player already exists"),
                Left: _ =>
                {
                    _players.Add(entity);
                    return entity;
                });
        
        public Either<Exception,Player> Delete(string id) 
            => Get(id).Match<Either<Exception, Player>>(
                Right: Remove, 
                Left: er => new Exception(er.Message));

        private Either<Exception, Player> Remove(Player player)
            => Try.lift(() => _players.Remove(player)).Run()
                .Match<Either<Exception, Player>>(
                    Succ: _ => player,
                    Fail: er => new Exception(er.Message));

        public Either<Exception,Player> Get(string id) 
            => Try.lift(() 
                    => _players.Single(p => p.HardwareID == id)).Run()
                .Match<Either<Exception, Player>>(
                    Succ: p => p,
                    Fail: er => new Exception(er.Message));

        public IEnumerable<Player> GetAll() => _players;

        public Either<Exception,Player> Update(Player entity)
            => Get(entity.HardwareID).Match<Either<Exception, Player>>(
                Right: p =>
                {
                    var index = _players.IndexOf(p);
                    _players[index] = entity;
                    return p;
                },
                Left: ex => ex);
    }
}
