using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Ycg.Extension
{
    public static class DataTableExtension
    {
        public static T GenerateInfo<T>(this DataTable dataTable) where T : class,new()
        {
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0].GenerateInfo<T>();
            }
            return new T();
        }

        public static IList<T> GenerateInfos<T>(this DataTable dataTable) where T : class,new()
        {
            return dataTable.GenerateInfos<T>(null);
        }

        public static IList<T> GenerateInfos<T>(this DataTable dataTable, Func<DataRow, T> func) where T : class,new()
        {
            IList<T> list = new List<T>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                PropertyInfo[] propertyInfos = typeof(T).GetProperties();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    list.Add(func == null ? dataRow.GenerateInfo<T>(propertyInfos) : dataRow.GenerateInfo(func));
                }
            }
            return list;
        }
    }
}