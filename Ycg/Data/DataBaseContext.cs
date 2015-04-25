namespace Ycg.Data
{
    public static class DataBaseContext
    {
        public static IDBHelper DBInstance
        {
            get
            {
                return DataBaseManager.Instance();
            }
        }
    }
}
