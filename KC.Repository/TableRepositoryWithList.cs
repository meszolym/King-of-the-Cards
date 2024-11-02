using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using LanguageExt;
using LanguageExt.Common;

namespace KC.Repository
{
    internal class TableRepositoryWithList : IRepository<Table, Guid>
    {
        private readonly List<Table> _tables = [];
        public Fin<Table> Get(Guid id)
            => Try.lift(()
                    => _tables.Single(p => p.TableId == id)).Run()
                .Match<Fin<Table>>(
                    Succ: t => t,
                    Fail: er => Error.New(er.Message));

        public IEnumerable<Table> GetAll() => _tables;

        public Fin<Table> Add(Table entity) 
            => Get(entity.TableId).Match<Fin<Table>>(
            Succ: _ => Error.New("Table already exists"),
            Fail: _ =>
            {
                _tables.Add(entity);
                return entity;
            });

        public Fin<Table> Update(Table entity) =>
            Get(entity.TableId).Match<Fin<Table>>(
                Succ: t =>
                {
                    var index = _tables.IndexOf(t);
                    _tables[index] = entity;
                    return t;
                },
                Fail: er => er);

        public Fin<Table> Delete(Guid id) => Get(id).Match<Fin<Table>>(
            Succ: Remove,
            Fail: er => Error.New(er.Message));

        private Fin<Table> Remove(Table table)
            => Try.lift(() => _tables.Remove(table)).Run()
                .Match<Fin<Table>>(
                    Succ: _ => table,
                    Fail: er => Error.New(er.Message));
    }
}
