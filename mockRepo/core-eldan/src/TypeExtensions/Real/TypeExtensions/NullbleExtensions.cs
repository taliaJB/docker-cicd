using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.TypeExtensions
{
    public static class NullbleExtensions
    {
        public static string ToNullLessStringNTOnly<T>(this T source)
        {
            return source.ToNullLessStringNTOnly("null");
        }

        public static string ToNullLessStringNTOnly<T>(this T source, string nullValue)
        {
            Type CurrentType = typeof(T);
            Type UnderlyingType = Nullable.GetUnderlyingType(CurrentType);
            if (UnderlyingType == null)
                throw new Exception("NullbleExtensions.ToNullLessStringNTOnly: Source is not nullble");

            return (source == null) ? nullValue : source.ToString();
        }
    }
}
