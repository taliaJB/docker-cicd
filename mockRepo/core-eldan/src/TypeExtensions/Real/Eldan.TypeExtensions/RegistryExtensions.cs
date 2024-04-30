using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Eldan.TypeExtensions
{
    public static class RegistryExtensions
    {
        public static bool TryGetRegistryValue(string keyName, string valueName, out string registryValue)
        {
            registryValue = GetRegistryValue(keyName, valueName);

            return registryValue != null;
        }

        public static string GetRegistryValue(this string keyName)
        {
            return GetRegistryValue(keyName, null);
        }

        public static string GetRegistryValue(this string keyName, string valueName)
        {
            valueName.ToNullLessString();

            object val = Registry.GetValue(keyName, valueName, null);
            if (val == null)
            {
                return null;
            }

            return val.ToString();
        }
    }
}
