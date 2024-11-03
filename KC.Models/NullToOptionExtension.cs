using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Models
{
    internal static class NullToOptionExtension
    {
        public static Option<T> ToOption<T>(this T? value)
        {
            return value == null ? Option<T>.None : Option<T>.Some(value);
        }
    }
}
