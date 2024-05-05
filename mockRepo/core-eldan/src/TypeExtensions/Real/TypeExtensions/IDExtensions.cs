using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Eldan.TypeExtensions
{
    public static class IDExtensions
    {
        public static void ValidateID(this string id) => id.ValidateID(nameof(id));

        public static void ValidateID(this string id, string name)
        {
            if (!id.IsIDValid())
                throw new ArgumentException(string.Format("IDExtensions.ValidateID - {0}: '{1}' is invalid", 
                    name.MaskNull(), id.MaskNull()));
        }

        public static bool IsIDValid(this string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            if (!Regex.IsMatch(id, @"^\d{5,9}$"))
                return false;

            long.TryParse(id, out long longID);
            if (longID == 0)
                return false;

            // number is too short - add leading 0000
            if (id.Length < 9)
            {
                while (id.Length < 9)
                {
                    id = '0' + id;
                }
            }

            //validate
            int mone = 0;
            int incNum;
            for (int i = 0; i < 9; i++)
            {
                incNum = Convert.ToInt32(id[i].ToString());
                incNum *= (i % 2) + 1;
                if (incNum > 9)
                    incNum -= 9;
                mone += incNum;
            }
            if (mone % 10 == 0)
                return true;
            else
                return false;
        }
    }
}
