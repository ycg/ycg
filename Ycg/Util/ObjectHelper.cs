using System;
using System.Net.Configuration;
using System.Reflection;

namespace Ycg.Util
{
    public static class ObjectHelper
    {
        public static bool IsNull(this object obj)
        {
            return (obj == null || obj == DBNull.Value);
        }

        public static bool IsNotNull(this object obj)
        {
            return (obj != null && obj != DBNull.Value);
        }

        public static T CreateInstance<T>(string fullName)
        {
            return (T)Assembly.Load("Ycg").CreateInstance(fullName);
        }

        public static T CreateInstance<T>(string nameSpace, string fullName)
        {
            return CreateInstance<T>(nameSpace, fullName, null);
        }

        public static T CreateInstance<T>(string nameSpace, string fullName, object[] parameters)
        {
            return (T)Assembly.Load(nameSpace).CreateInstance(fullName, false, BindingFlags.CreateInstance, null, parameters, null, null);
        }
    }
}