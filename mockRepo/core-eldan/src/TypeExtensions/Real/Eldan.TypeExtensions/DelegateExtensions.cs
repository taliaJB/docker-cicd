using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.TypeExtensions
{
    public static class DelegateExtensions
    {
        // Extension method for Delegate type without return value
        public static void ExecuteDelegate(this Delegate method, params object[] parameters)
        {
            method.DynamicInvoke(parameters);
        }

        // Extension method for Delegate type with return value
        public static object ExecuteDelegateWithReturn(this Delegate method, params object[] parameters)
        {
            return method.DynamicInvoke(parameters);
        }
    }
}
