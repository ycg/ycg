namespace Ycg.Data
{
    public sealed class BulkCopyColumnMapping 
    {
        private string _sourceColumnName = string.Empty;
        private string _destinationColumnName = string.Empty;

        public BulkCopyColumnMapping() { }

        /// <summary>
        ///  Initinal Column Mapping.
        /// </summary>
        /// <param name="sourceName">Source Table Column Name.</param>
        /// <param name="destinationColumnName">DataBase Table Column Name.</param>
        public BulkCopyColumnMapping(string sourceName, string destinationColumnName)
        {
            this._sourceColumnName = sourceName;
            this._destinationColumnName = destinationColumnName;
        }

        public string SourceColumnName
        {
            get { return this._sourceColumnName; }
            set { this._sourceColumnName = value; }
        }

        public string DestinationColumnName
        {
            get { return this._destinationColumnName; }
            set { this._destinationColumnName = value; }
        }
    }
}
