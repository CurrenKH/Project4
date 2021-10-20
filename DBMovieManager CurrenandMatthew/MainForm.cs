using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DBMovieManager_CurrenandMatthew
{
    public partial class MainForm : Form
    {
        MySqlConnection dbConnection;

        public MainForm()
        {

            //This method sets up a connection to a MySQL database
            SetDBConnection("127.0.0.1", "3306", "CurrenH", "dfcg22r", "oop");

            checkDBServerVersion();

            InitializeComponent();
        }

        private void SetDBConnection(string serverAddress, string serverPort, string username, string passwd, string dbName)
        {

            // For connection testing purposes
            //string conectionString = "server=127.0.0.1; user=user1; database=test; port=3306; password=oop2; SSL Mode=None";

            string connectionString = "server=" + serverAddress + "; port=" + serverPort + "; user=" + username + "; password=" + passwd + "; database=" + dbName + "; SSL Mode=None;";

            Console.WriteLine(connectionString);

            dbConnection = new MySqlConnection(connectionString);

        }

        private void checkDBServerVersion()
        {
            try
            {
                //Before sending queries to the database, the connection must be opened
                dbConnection.Open();

                Console.WriteLine("DB Connection OK!!!");

                //This is a string representing the SQL query to execute in the db
                string sqlQuery = "SELECT version()";

                //This is the actual SQL command containing the query to be executed
                MySqlCommand dbCommand = new MySqlCommand(sqlQuery, dbConnection);

                //This variable stores the result of the SQL query sent to the db
                string dbServerVersion = dbCommand.ExecuteScalar().ToString();

                //After executing the query(ies) in the db, the connection must be closed
                dbConnection.Close();

                Console.WriteLine("\n----------------------");

                Console.WriteLine("DB Server version: " + dbServerVersion);
            }
            catch (MySqlException ex)
            {
                // if the db connection is still open, it must be closed, otherwise, further attemps to open the connection again will fail
                if (dbConnection.State.ToString() == "Open")
                {
                    dbConnection.Close();
                }

                Console.WriteLine(ex.Message);
            }

        }
    }
}
