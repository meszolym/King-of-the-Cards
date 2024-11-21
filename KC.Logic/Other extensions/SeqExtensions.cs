using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.App.Logic.Other_extensions
{
    public static class SeqExtensions
    {
        public static Seq<T> AddIf<T>(this Seq<T> seq, bool condition, T item) => condition ? seq.Add(item) : seq;
    }
}
