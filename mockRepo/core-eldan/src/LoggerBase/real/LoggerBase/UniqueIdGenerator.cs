using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.Logger
{
    internal class UniqueIdGenerator
    {
        private static Random random = new Random();

        public static string GenerateUniqueId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder uniqueId = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                uniqueId.Append(chars[random.Next(chars.Length)]);
            }

            return uniqueId.ToString();
        }
    }
}
