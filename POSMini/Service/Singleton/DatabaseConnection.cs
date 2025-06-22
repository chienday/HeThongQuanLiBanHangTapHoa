using System;
using System.Data.SqlClient;

namespace POSMini.Service.Singleton
{
    public sealed class DatabaseConnection
    {
        private static DatabaseConnection instance = null;
        private static readonly object locks = new object();
        private readonly string connectionString;

        private DatabaseConnection()
        {
            connectionString = @"Server=LAPTOP-BAR8P7OI\SQLEXPRESS;Initial Catalog=Posmini;Integrated Security=True;";
        }

        public static DatabaseConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locks)
                    {
                        if (instance == null)
                            instance = new DatabaseConnection();
                    }
                }
                return instance;
            }
        }

       
        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
