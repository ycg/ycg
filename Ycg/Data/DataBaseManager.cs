/*
 
 * 2013.12.13
 * 
 *      使用单例模式，并调用工厂方法获取相应的数据库实例。
 * 
 */

namespace Ycg.Data
{
    internal static class DataBaseManager
    {
        private static IDBHelper _instance;
        private static readonly object _obj = new object();

        static DataBaseManager()
        { }

        public static IDBHelper Instance()
        {
            if (_instance == null)
            {
                lock (_obj)
                {
                    return _instance ?? (_instance = DataBaseFactory.DBInstance());
                }
            }
            return _instance;
        }
    }
}
