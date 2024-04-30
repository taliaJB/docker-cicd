using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eldan.TypeExtensions
{
    public static class ListExtensions
    {
        public static bool Exists(this List<string> source, string value, StringComparison comparison)
        {
            if (value == null)
                throw new Exception("Value must not contains null");

            return (source.FindAll(x => x.Equal(value, comparison)).Count() > 0);
        }
    }
}
