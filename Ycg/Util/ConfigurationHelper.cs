using System.Configuration;

namespace Ycg.Util
{
    public static class ConfigurationHelper
    {
        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key].Trim();
        }

        public static void SetValue(string key, string value)
        {
            ConfigurationManager.AppSettings.Set(key, value);
        }

        public static void Add(string key, string value)
        {
            ConfigurationManager.AppSettings.Add(key, value);
        }

        public static void Remove(string key)
        {
            ConfigurationManager.AppSettings.Remove(key);
        }
    }
}