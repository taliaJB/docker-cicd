using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.TypeExtensions
{
    public static class EnumExtensions
    {
        public static T DefinedCast<T>(this object enumValue) where T : struct, IConvertible
        {
            if (!Enum.IsDefined(typeof(T), enumValue))
                throw new InvalidCastException(string.Format("EnumExtensions.DefinedCast - '{0}' is not a defined value for enum type: '{1}'", 
                                                        enumValue, typeof(T).FullName));
                                               
            return (T)enumValue;
        }
    }
}
