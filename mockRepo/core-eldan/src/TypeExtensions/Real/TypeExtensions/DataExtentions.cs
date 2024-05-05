using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Eldan.TypeExtensions
{
    public static class DataExtentions
    {
        public static T GetFieldValue<T>(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return default(T);

            return row.Field<T>(columnName);
        }
    }
}
