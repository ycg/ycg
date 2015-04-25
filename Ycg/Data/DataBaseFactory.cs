/*
 
 * 2013.12.13
 * 
 *  1.读取配置文件内容，并通过反射生成对应的数据库实例
 *  
 * 2013.12.27
 * 
 *  1. 首先先从AppSeeting中读取连接字符串
 *  2. 如果没有在从具体配置项里面进行读取
 *  3. 如果没有指定具体数据库实例名，就默认初始化SQL Server的实例
 *  
 * 2013.12.29
 * 
 *  1. 一个很重要的知识点 - 一定要记住
 *  
 *  <section 
                    name="section name"
                    type="configuration section handler class, assembly file name, version, culture, public key token"
                    allowDefinition= "Everywhere|MachineOnly|MachineToApplication|MachineToWebRoot" 
                    allowLocation="True|False" 
                    restartOnExternalChanges="True|False" 
    />
 * 
 *  其中[onfiguration section handler class] 也就是自己定义的Section类，这个一定要有清楚的认识
 * 
 */

using System.Configuration;

using Ycg.Data.Configuration;
using Ycg.Data.SQLServer;
using Ycg.Util;

namespace Ycg.Data
{
    internal static class DataBaseFactory
    {
        internal static readonly DataBaseConfigurationInfo DBConfigurationInfo = GetDefaultDataBaseConfigurationInfo() ?? GetDataBaseConfigurationInfo();

        internal static IDBHelper DBInstance()
        {
            return ObjectHelper.CreateInstance<IDBHelper>(DBConfigurationInfo.Namespace, DBConfigurationInfo.FullName, new object[] { DBConfigurationInfo.ConnectionString });
        }

        private static DataBaseConfigurationInfo GetDataBaseConfigurationInfo()
        {
            DataBaseConfigurationSection dataBaseConfigurationSection = (DataBaseConfigurationSection)ConfigurationManager.GetSection("database");
            return new DataBaseConfigurationInfo
            {
                FullName = dataBaseConfigurationSection.Name,
                Namespace = dataBaseConfigurationSection.Namespace,
                ConnectionString = GetAsyncConnectionString(dataBaseConfigurationSection.ConnectionString.ConnectionString, dataBaseConfigurationSection.ConnectionString.IsAsync),
                IsAsync = dataBaseConfigurationSection.ConnectionString.IsAsync
            };
        }

        private static DataBaseConfigurationInfo GetDefaultDataBaseConfigurationInfo()
        {
            string connstriongString = ConfigurationManager.AppSettings["ConnectionString"];
            if (string.IsNullOrEmpty(connstriongString)) return null;
            return new DataBaseConfigurationInfo
            {
                Namespace = DataBaseDefaultConfiguration.Namespace,
                FullName = DataBaseDefaultConfiguration.FullName,
                ConnectionString = GetAsyncConnectionString(connstriongString, DataBaseDefaultConfiguration.IsAsync),
                IsAsync = DataBaseDefaultConfiguration.IsAsync
            };
        }

        private static string GetAsyncConnectionString(string connectionString, bool isAsync)
        {
            return isAsync ? string.Concat(connectionString, connectionString.EndsWith(";") 
                                        ? string.Empty 
                                        : ";", DataBaseDefaultConfiguration.StrAsync)
                                : connectionString;
        }
    }
}
