using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Ycg.Extension
{
    public static class DataReaderExtension
    {
        public static IList<T> GenerateInfos<T>(this IDataReader dataReader) where T : class,new()
        {
            if (dataReader == null || dataReader.IsClosed) return null;

            IList<T> list = new List<T>();
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            Dictionary<string, PropertyInfo> nameAndPropertyInfo = propertyInfos.ToDictionary(propertyInfo => propertyInfo.Name);
            while (dataReader.Read())
            {
                T t = new T();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    string columnName = dataReader.GetName(i);
                    if (nameAndPropertyInfo.ContainsKey(columnName))
                    {
                        SetValue(t, nameAndPropertyInfo[columnName], dataReader[i]);
                    }
                }
                list.Add(t);
            }

            return list;
        }

        public static void SetValue<T>(T t, PropertyInfo propertyInfo, object value) where T : class,new()
        {
            if (value != null && value != DBNull.Value)
            {
                if (propertyInfo.PropertyType.IsEnum)
                {
                    //propertyInfo.SetValue(t, Enum.Parse(propertyInfo.PropertyType, value.ToString()), null);
                    propertyInfo.SetValue(t, Enum.ToObject(propertyInfo.PropertyType, value.ToInt32()), null);
                }
                else
                {
                    switch (propertyInfo.PropertyType.Name)
                    {
                        case "Int32":
                            propertyInfo.SetValue(t, value.ToInt32(), null);
                            break;
                        case "DateTime":
                            propertyInfo.SetValue(t, value.ToDateTime(), null);
                            break;
                        case "Boolean":
                            propertyInfo.SetValue(t, value.ToBool(), null);
                            break;
                        case "Double":
                            propertyInfo.SetValue(t, value.ToDouble(), null);
                            break;
                        case "Byte[]":
                            propertyInfo.SetValue(t, value.ToBytes(), null);
                            break;
                        default:
                            propertyInfo.SetValue(t, value, null);
                            break;
                    }
                }
            }
        }
    }
}