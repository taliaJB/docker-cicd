using Eldan.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTE.ObjectClone
{
    public enum EnmMySourceEnum
    {
        SourceValA,
        SourceValB
    }

    class MySource
    {

        public string GetString(int MyVal)
        {
            return MyVal.ToString();
        }

    }
}
