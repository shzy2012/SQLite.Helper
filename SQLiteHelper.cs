using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace SQLite.Helper
{
    public class SQLiteHelper
    {
        // Variables
        private SQLiteConnection connection;
        public bool generalError = false; // If and general error has occurred
        public bool sqlError = false; // If and sql error has occured

        /// <summary>
        /// Constructor
        /// Checks if the database exists (if isn't creating)
        /// Creates the connnection to the database
        /// </summary>
        /// <param name="__database">Database Path</param>
        /// <param name="__readOnly">Open database in read only (Ignored when creating)</param>
        /// <param name="__create">Create new database</param>
        /// <param name="__connectionStr">Custom connection string</param>
        public SQLiteHelper(string __database, bool __readOnly = true, bool __create = false, string __connectionStr = null)
        {
            // Checks if the database exists
            if (!File.Exists(__database) && !__create)
            {
                MessageBox.Show("Database" + __database + "doesn't exists", "Database not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                generalError = true;
            }

            string connectionStr = __connectionStr;
            // Generates the connections string
            if (__connectionStr == null)
            {
                if (__create)
                    connectionStr = "Data Source=" + __database + "; Version=3";
                else
                {
                    connectionStr = "Data Source=" + __database + "; Version=3; FailIfMissing=True";

                    // If the connection is read only, appends the read only argument
                    if (__readOnly)
                        connectionStr += "; ReadOnly = True";
                }
            }

            // Creates the connection
            connection = new SQLiteConnection(connectionStr);
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <param name="__dropSql">SQL to drop current tables</param>
        public void CreateDb(string __sql, string __dropSql = null)
        {
            // Check if accored any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Sets the sql error to false
            sqlError = false;
            try
            {
                // Open a connection to the database
                connection.Open();

                // Drop the current values
                if (__dropSql != null)
                {
                    SQLiteCommand cmdDrop = new SQLiteCommand(__dropSql, connection);
                    cmdDrop.ExecuteNonQuery();
                }

                // Creates the database
                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                cmd.ExecuteNonQuery();

                // Close the connection and return the value
                connection.Close();
                return;
            }
            catch (SQLiteException error)
            {
                // Shows the error message
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
            }
        }

        /// <summary>
        /// Writes something on the database
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        public void Write(string __sql)
        {

            // Check if accored any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Sets the sql error to false
            sqlError = false;
            try
            {
                // Open a connection to the database
                connection.Open();

                // Get the values
                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                cmd.ExecuteNonQuery();

                // Close the connection and return the value
                connection.Close();
                return;
            }
            catch (SQLiteException error)
            {
                // Shows the error message
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
            }
        }

        /// <summary>
        /// Get a integer value from the database
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <returns>Returns and integer</returns>
        public int GetInt(string __sql)
        {
            // Check if accored any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }

            // Sets the sql error to false
            sqlError = false;
            try
            {
                // Open a connection to the database
                connection.Open();

                // Get the values
                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                int result = Convert.ToInt32(cmd.ExecuteScalar());

                // Close the connection and return the value
                connection.Close();
                return result;
            }
            catch (SQLiteException error)
            {
                // Shows the error message
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
                return 0;
            }
        }

        /// <summary>
        /// Get a string from the database
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <returns></returns>
        public string GetString(string __sql)
        {
            // Check if accored any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "";
            }

            // Sets the sql error to false
            sqlError = false;
            try
            {
                // Open a connection to the database
                connection.Open();

                // Get the values
                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                string result = Convert.ToString(cmd.ExecuteScalar());

                // Close the connection and return the value
                connection.Close();
                return result;
            }
            catch (SQLiteException error)
            {
                // Shows the error message
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
                return "";
            }
        }
    }
}
