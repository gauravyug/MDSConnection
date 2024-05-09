using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace DatabaseConnectionApp
{
    public partial class MainForm : Form, IDisposable
    {
        private DatabaseConnector _connector;
        private bool _disposed = false;
        private SqlConnection _connection;
        public MainForm()
        {
            InitializeComponent();
            _connector = new DatabaseConnector("Data Source=idcdacfx.database.windows.net;Initial Catalog=master;Persist Security Info=False;User ID=gayadav@microsoft.com;Pooling=False;Multiple Active Result Sets=False;Encrypt=True;Trust Server Certificate=True;Authentication=ActiveDirectoryInteractive;Application Name=\"Microsoft SQL Server Data Tools, SQL Server Object Explorer\";Command Timeout=0");
        }


        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            string serverName = ServerTextBox.Text;
            string connectionString = $"Server={serverName};";
            try
            {
              
                await ConnectToDatabaseAsync(serverName);
                _connection = new SqlConnection(connectionString);
                _connection.Open();
                MessageBox.Show("Connection successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Perform your database operations here...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task ConnectToDatabaseAsync(string serverName)
        {
            string connectionString = $"Server={serverName};";
            try
            {
                _connection = new SqlConnection(connectionString);
                await TryOpenConnectionWithRetryAsync(_connection);

                MessageBox.Show("Connection successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Perform your database operations here...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TryOpenConnectionWithRetryAsync(SqlConnection connection)
        {
            const int maxRetryAttempts = 3;
            const int retryIntervalSeconds = 5;
            int retryAttempts = 0;

            while (retryAttempts < maxRetryAttempts)
            {
                try
                {
                    await connection.OpenAsync();
                    return; // Connection successful, exit retry loop
                }
                catch (SqlException ex)
                {
                    retryAttempts++;
                    if (retryAttempts >= maxRetryAttempts)
                        throw; // Max retry attempts reached, rethrow the exception

                    // Log or handle the exception if needed
                    Console.WriteLine($"Connection attempt {retryAttempts} failed. Retrying in {retryIntervalSeconds} seconds...");

                    // Wait before retrying
                    await Task.Delay(retryIntervalSeconds * 1000);
                }
            }
        }
    
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connector != null)
                    {
                        _connector.Dispose();
                    }
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
       
    }
}
