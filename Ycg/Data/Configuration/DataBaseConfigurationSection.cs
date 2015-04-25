using System.Configuration;

namespace Ycg.Data.Configuration
{
    public class DataBaseConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("name", DefaultValue = DataBaseDefaultConfiguration.FullName)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("namespace", DefaultValue = DataBaseDefaultConfiguration.Namespace)]
        public string Namespace
        {
            get { return (string)base["namespace"]; }
            set { base["namespace"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = false)]
        public DataBaseConnectionStringConfigurationElement ConnectionString
        {
            get
            {
                return (DataBaseConnectionStringConfigurationElement)base["connectionString"];
            }
        }
    }
}
