using System;
using Microsoft.Data.SqlClient;

namespace DatabaseConnectionApp
{
    public class DatabaseConnector : IDisposable
    {
        private readonly SqlConnection _connection;

        public DatabaseConnector(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
        }

        ~DatabaseConnector()
        {
            Dispose(false);
        }
    }
}
