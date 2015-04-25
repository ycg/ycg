namespace Ycg.Data
{
    public sealed class ParameterMappingInfo
    {
        public ParameterMappingInfo()
        {
        }

        public ParameterMappingInfo(string parameterName, object parameterValue)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
        }

        public string ParameterName { get; set; }

        public object ParameterValue { get; set; }
    }
}
