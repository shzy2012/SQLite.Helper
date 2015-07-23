/*
This file is part of SQLite.Helper.

SQLite.Helper is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SQLite.Helper is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with SQLite.Helper.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Data;

namespace SQLite.Helper
{
    public class SQLiteHelper
    {
        // Variables
        private SQLiteConnection connection;
        public bool generalError { get; private set; } // If and general error has occurred
        public bool sqlError { get; private set; } // If and sql error has occured

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

            // Generates the connection string
            string connectionStr = __connectionStr;
            if (connectionStr == null)
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
            // Check if occurred any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            sqlError = false;
            try
            {
                connection.Open();

                // Drop the current values
                if (__dropSql != null)
                {
                    SQLiteCommand cmdDrop = new SQLiteCommand(__dropSql, connection);
                    cmdDrop.ExecuteNonQuery();
                }

                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException error)
            {
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Writes something on the database
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        public void Write(string __sql)
        {

            // Check if occurred any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Sets the sql error to false
            sqlError = false;
            try
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException error)
            {
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Get a integer value from the database
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <returns>Returns and integer</returns>
        public int GetInt(string __sql)
        {
            int result = 0;

            // Check if occurred any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }

            sqlError = false;
            try
            {
                connection.Open();

                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SQLiteException error)
            {
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
            }
            finally
            {
                connection.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Get a string from the database
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <returns>Returns a string</returns>
        public string GetString(string __sql)
        {
            string result = "";
            // Check if occurred any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "";
            }


            sqlError = false;
            try
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(__sql, connection);
                result = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (SQLiteException error)
            {
                // Shows the error message
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
            }
            finally
            {
                connection.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Gets table values inside a dataset
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <param name="__table">Name of the table</param>
        /// <returns>Returns a dataset</returns>
        public DataSet GetData(string __sql, string __table)
        {
            DataSet data = new DataSet();

            // Check if occurred any error
            if (generalError)
            {
                MessageBox.Show("An error has occurred in the past", "Can't do that", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return data;
            }

            sqlError = false;
            try
            {
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(__sql, connection);
                adapter.Fill(data, __table);
                adapter.Dispose();
                connection.Dispose();
            }
            catch (SQLiteException error)
            {
                // Shows the error message
                MessageBox.Show("SQLite Error: " + error.Message, "SQLite Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sqlError = true;
            }
            finally
            {
                connection.Dispose();
            }

            return data;
        }
    }
}
