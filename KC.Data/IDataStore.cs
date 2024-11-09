﻿using KC.Models.Interfaces;
using LanguageExt;

namespace KC.Data;

public interface IDataStore<TVal, TKey> where TVal : class, IIdentityBearer<TKey> where TKey : IComparable
{
    IEnumerable<TVal> GetAll();
    Fin<Unit> Add(TVal item);
    Fin<Unit> Remove(TKey id);
    Fin<TVal> Get(TKey id);
    Fin<Unit> Update(TVal item);
}