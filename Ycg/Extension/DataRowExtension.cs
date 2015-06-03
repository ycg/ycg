using System;
using System.Data;
using System.Reflection;

namespace Ycg.Extension
{
    public static class DataRowExtension
    {
        public static T GenerateInfo<T>(this DataRow dataRow) where T : class ,new()
        {
            if (dataRow == null) throw new ArgumentNullException();
            return GenerateInfo<T>(dataRow, typeof (T).GetProperties());
        }

        public static T GenerateInfo<T>(this DataRow dataRow, PropertyInfo[] propertyInfos) where T : class ,new()
        {
            if (dataRow == null) throw new ArgumentNullException();
            T t = new T();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                string propertyName = propertyInfo.Name;
                if (dataRow.Table.Columns.Contains(propertyName))
                {
                    DataReaderExtension.SetValue(t, propertyInfo, dataRow[propertyName]);
                }
            }
            return t;
        }

        public static T GenerateInfo<T>(this DataRow dataRow, Func<DataRow, T> func) where T : class,new()
        {
            if (dataRow == null) return new T();
            return func(dataRow);
        }
    }
} 