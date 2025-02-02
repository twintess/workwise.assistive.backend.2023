namespace db.mrds.net
{
    public class DatabaseParameter
    {
        public DatabaseParameter(string connectionString, ConnectionType connectionType)
        {
            _connectionString = connectionString;
            _connectionType = connectionType;
        }

        private readonly string _connectionString;
        private readonly ConnectionType _connectionType;

        public string? GeConnectionString()
        {
            return _connectionString;
        }

        public ConnectionType GetConnectionType()
        {
            return _connectionType;
        }
    }
}
