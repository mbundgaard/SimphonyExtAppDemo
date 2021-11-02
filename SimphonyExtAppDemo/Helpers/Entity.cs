using System;
using System.Data;
using System.Reflection;

namespace SimphonyExtAppDemo.Helpers
{
    public static class Entity
    {
        public static T CreateInstance<T>(DataRow row)
        {
            var output = default(T);

            if (row == null)
                return output;

            output = Activator.CreateInstance<T>();

            foreach (DataColumn column in row.Table.Columns)
            {
                if (!output.GetType().IsClass)
                    return (T)row[column.Ordinal];

                var prop = output.GetType().GetProperty(column.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                    throw new Exception("No matching property found for column " + column.ColumnName);
                try
                {
                    var value = row[column.Ordinal];

                    if (value != DBNull.Value)
                        prop.SetValue(output, value, null);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to set value for property " + column.ColumnName, e);
                }
            }
            return output;
        }
    }
}