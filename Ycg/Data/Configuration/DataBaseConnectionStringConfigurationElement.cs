using System.Configuration;

namespace Ycg.Data.Configuration
{
    public class DataBaseConnectionStringConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsKey = true, IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }

        [ConfigurationProperty("isAsync", DefaultValue = DataBaseDefaultConfiguration.IsAsync)]
        public bool IsAsync
        {
            get { return (bool)base["isAsync"]; }
            set { base["isAsync"] = value; }
        }
    }
}
