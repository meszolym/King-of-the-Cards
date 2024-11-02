using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using LanguageExt;

namespace KC.Repository
{
    internal class TableRepositoryWithList : IRepository<Table, Guid>
    {
        private readonly List<Table> _tables = [];
        public Either<Exception, Table> Get(Guid id)
            => Try.lift(()
                    => _tables.Single(p => p.TableId == id)).Run()
                .Match<Either<Exception, Table>>(
                    Succ: t => t,
                    Fail: er => new Exception(er.Message));

        public IEnumerable<Table> GetAll() => _tables;

        public Either<Exception, Table> Add(Table entity) 
            => Get(entity.TableId).Match<Either<Exception, Table>>(
            Right: _ => new Exception("Table already exists"),
            Left: _ =>
            {
                _tables.Add(entity);
                return entity;
            });

        public Either<Exception, Table> Update(Table entity) =>
            Get(entity.TableId).Match<Either<Exception, Table>>(
                Right: t =>
                {
                    var index = _tables.IndexOf(t);
                    _tables[index] = entity;
                    return t;
                },
                Left: ex => ex);

        public Either<Exception, Table> Delete(Guid id) => Get(id).Match<Either<Exception, Table>>(
            Right: Remove,
            Left: er => new Exception(er.Message));

        private Either<Exception, Table> Remove(Table table)
            => Try.lift(() => _tables.Remove(table)).Run()
                .Match<Either<Exception, Table>>(
                    Succ: _ => table,
                    Fail: er => new Exception(er.Message));
    }
}
