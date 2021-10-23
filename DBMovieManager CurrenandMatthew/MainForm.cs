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
        //  Constants to use when creating database connections
        private const string dbServerHost = "127.0.0.1";
        private const string dbUsername = "CurrenH";
        private const string dbPassword = "dfcg22r";
        private const string dbName = "oop";

        MySqlConnection dbConnection;

        public MainForm()
        {

            //This method sets up a connection to a MySQL database
            SetDBConnection("127.0.0.1", "CurrenH", "dfcg22r", "oop");

            checkDBServerVersion();

            getMoviesFromDB();

            getGenresFromDB();

            InitializeComponent();
        }
        List<Movie> Movies = new List<Movie>();
        List<Genre> Genres = new List<Genre>();
        List<Member> Members = new List<Member>();

        private void SetDBConnection(string serverAddress, string username, string password, string dbName)
        {

            // For connection testing purposes
            //string conectionString = "server=127.0.0.1; user=user1; database=test; port=3306; password=oop2; SSL Mode=None";

            string connectionString = "server=" + serverAddress + "; user=" + username + "; password=" + password + "; database=" + dbName + "; SSL Mode=None;";

            Console.WriteLine(connectionString);

            dbConnection = new MySqlConnection(connectionString);

        }

        private MySqlConnection CreateDBConnection(string serverAddress, string username, string passwd, string dbName)
        {
            string connectionString = "Host=" + serverAddress + "; Username=" + username + "; Password=" + passwd + "; Database=" + dbName + ";";

            dbConnection = new MySqlConnection(connectionString);

            return dbConnection;
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

        private void getMoviesFromDB()
        {
            //movieListView.Items.Clear();

            Movies.Clear();

            Movie currentMovie;

            //Connect to the database before sending commands
            dbConnection.Open();

            //This is a string representing the SQL query to execute in the db            
            string sqlQuery = "SELECT * FROM movie;";
            Console.WriteLine("SQL Query: " + sqlQuery);

            //This is the actual SQL containing the query to be executed
            MySqlCommand dbCommand = new MySqlCommand(sqlQuery, dbConnection);

            //This variable stores the result of the SQL query sent to the db
            MySqlDataReader dataReader = dbCommand.ExecuteReader();

            Console.WriteLine("\n=================");

            try
            {
                //Read each line present in the dataReader
                while (dataReader.Read())
                {

                    //Create a new Movie and setup its info
                    currentMovie = new Movie();

                    currentMovie.Id = dataReader.GetInt32(0);
                    currentMovie.Title = dataReader.GetString(1);
                    currentMovie.Year = dataReader.GetInt32(2);
                    currentMovie.Length = dataReader.GetInt32(3);
                    currentMovie.Director = dataReader.GetString(4);
                    currentMovie.Rating = dataReader.GetDouble(4);

                    currentMovie.Genres = LoadMovieGenres(currentMovie.Id);

                    if (dataReader.GetString(5) == "")
                    {
                        currentMovie.ImagePath = @"images\noimage.jpg";
                        movieImageList.Images.Add(Image.FromFile(currentMovie.ImagePath.ToString()));
                        Movies.Add(currentMovie);
                    }

                    else
                    {
                        currentMovie.ImagePath = dataReader.GetString(5);
                        movieImageList.Images.Add(Image.FromFile(currentMovie.ImagePath.ToString()));
                        Movies.Add(currentMovie);
                    }

                    Console.WriteLine("image = " + currentMovie.ImagePath);


                    Console.WriteLine("Title = " + currentMovie.Title);

                    //Movies.Add(currentMovie);

                }
            }
            catch
            {
                //MessageBox.Show("ERROR HERE");
            }

            //After executing the query(ies) in the db, the connection must be closed
            dbConnection.Close();

            DisplayMovies();
        }

        private List<Genre> LoadMovieGenres(int movieID)
        {
            //The following Connection, Command and DataReader objects will be used to access the jt_genre_movie table
            MySqlConnection dbConnection2 = CreateDBConnection(dbServerHost, dbUsername, dbPassword, dbName);
            MySqlCommand dbCommand2;
            MySqlDataReader dataReader2;
            MySqlConnection dbConnection3 = CreateDBConnection(dbServerHost, dbUsername, dbPassword, dbName);
            MySqlCommand dbCommand3;
            MySqlDataReader dataReader3;


            string currentGenreCode;

            Genre currentGenre;

            List<Genre> GenreList = new List<Genre>();

            dbConnection2.Open();

            string sqlQuery = "SELECT * FROM jt_genre_movie WHERE movie_id = " + movieID + ";";

            Console.WriteLine("sqlQuery = " + sqlQuery);

            dbCommand2 = new MySqlCommand(sqlQuery, dbConnection2);

            dataReader2 = dbCommand2.ExecuteReader();

            //While there are genre_codes in the dataReader2
            while (dataReader2.Read())
            {
                currentGenre = new Genre();

                currentGenreCode = dataReader2.GetString(0);

                //Open a connection to access the 'genre' table
                dbConnection3.Open();

                sqlQuery = "SELECT * FROM genre WHERE code = '" + currentGenreCode + "';";

                Console.WriteLine("sqlQuery = " + sqlQuery);

                dbCommand3 = new MySqlCommand(sqlQuery, dbConnection3);

                dataReader3 = dbCommand3.ExecuteReader();

                //Read a line from the 'genre' table
                dataReader3.Read();

                currentGenre.Code = dataReader3.GetString(0);
                currentGenre.Name = dataReader3.GetString(1);
                currentGenre.Description = dataReader3.GetString(2);

                Console.WriteLine("currentGenre = " + currentGenre.Code + " - " + currentGenre.Name + " - " + currentGenre.Description);

                //dbCommand3.Dispose();
                //dataReader3.Close();

                GenreList.Add(currentGenre);

                dbConnection3.Close();
            }

            dbConnection2.Close();

            return GenreList;
        }

        private void DisplayMovies()
        {
            for (int i = 0; i < Movies.Count; i++)
            {
                movieListView.Items.Add(Movies[i].Title);
                movieListView.Items[i].SubItems.Add(Movies[i].Year.ToString());
                movieListView.Items[i].SubItems.Add(Movies[i].Length.ToString());
            }
        }

        private void getGenresFromDB()
        {
            Genre currentGenre;

            //Connect to the database before sending commands
            dbConnection.Open();

            //This is a string representing the SQL query to execute in the db            
            string sqlQuery = "SELECT * FROM genre;";
            Console.WriteLine("SQL Query: " + sqlQuery);

            //This is the actual SQL containing the query to be executed
            MySqlCommand dbCommand = new MySqlCommand(sqlQuery, dbConnection);

            //This variable stores the result of the SQL query sent to the db
            MySqlDataReader dataReader = dbCommand.ExecuteReader();

            Console.WriteLine("\n=================");

            //Read each line present in the dataReader
            while (dataReader.Read())
            {

                //Create a new Movie and setup its info
                currentGenre = new Genre();

                currentGenre.Code = dataReader.GetString(0);
                currentGenre.Name = dataReader.GetString(1);

                Console.WriteLine("Code = " + currentGenre.Code + "\n" + "Name = " + currentGenre.Name);

                Genres.Add(currentGenre);

            }

            //After executing the query(ies) in the db, the connection must be closed
            dbConnection.Close();

            //DisplayGenres();

        }

        /*private void DisplayGenres()
        {
            for (int i = 0; i < Genres.Count; i++)
            {
                genreListBox.Items.Add(Genres[i].Name);
            }
        }*/

        private void listViewColumnFormat()
        {
            //listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ColumnHeader columnHeader1 = new ColumnHeader();
            columnHeader1.Text = "Title";
            columnHeader1.TextAlign = HorizontalAlignment.Left;
            columnHeader1.Width = 140;
            movieListView.Columns.Add(columnHeader1);
            movieListView.View = View.Details;


            ColumnHeader columnHeader2 = new ColumnHeader();
            columnHeader2.Text = "Year";
            columnHeader2.TextAlign = HorizontalAlignment.Left;
            columnHeader2.Width = 60;
            movieListView.Columns.Add(columnHeader2);
            //listView1.View = View.Details;

            ColumnHeader columnHeader3 = new ColumnHeader();
            columnHeader3.Text = "Length";
            columnHeader3.TextAlign = HorizontalAlignment.Left;
            columnHeader3.Width = 80;
            movieListView.Columns.Add(columnHeader3);

            /*titleTextBox.Enabled = false;
            yearTextBox.Enabled = false;
            lengthTextBox.Enabled = false;
            genreTextBox.Enabled = false;
            pictureTextBox.Enabled = false;
            ratingTextBox.Enabled = false;*/
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            listViewColumnFormat();
        }
    }
}
