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
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SQLite.Helper
{
    /// <summary>
    /// Class to simplify connections to a SQLite database
    /// </summary>
    public class SQLiteHelper
    {
        /// <summary>
        /// Connection to the SQLite database
        /// </summary>
        public SQLiteConnection Connection { get; set; }

        /// <summary>
        /// If and general error has occurred 
        /// </summary>
        public bool GeneralError { get; private set; }

        /// <summary>
        /// If and sql error has occured 
        /// </summary>
        public bool SqlError { get; private set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string SqlErrorMessage { get; private set; }

        /// <summary>
        /// SQLite error number 
        /// </summary>
        public int SqlErrorNum { get; private set; }

        /// <summary>
        /// Doesn't create a connection
        /// </summary>
        public SQLiteHelper()
        {
            GeneralError = false;
            SqlError = false;
            SqlErrorMessage = "";
            SqlErrorNum = 0;
        }
        
        /// <summary>
        /// Checks if the database exists and creates the connnection to the database
        /// </summary>
        /// <param name="__Database">Database Path</param>
        public SQLiteHelper(string __Database)
        {
            GeneralError = false;
            SqlError = false;
            SqlErrorMessage = "";
            SqlErrorNum = 0;

            // Checks if the database exists
            if (!File.Exists(__Database))
            {
                GeneralError = true;
            }
            else
            {
                // Creates the connection
                Connection = new SQLiteConnection("Data Source=" + __Database + "; Version=3");
            }
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="__Sql">SQL statement</param>
        /// <param name="__DropSql">SQL to drop current tables</param>
        public void CreateDb(string __Sql, string __DropSql = null)
        {
            // Check if occurred any error
            if (GeneralError)
            {
                return;
            }

            SQLiteCommand SqlCmd = null;
            try
            {
                Connection.Open();

                // Drop the current values
                if (__DropSql != null)
                {
                    SqlCmd = new SQLiteCommand(__DropSql, Connection);
                    SqlCmd.ExecuteNonQuery();
                    SqlCmd.Dispose();
                }

                SqlCmd = new SQLiteCommand(__Sql, Connection);
                SqlCmd.ExecuteNonQuery();
                SqlError = false;
            }
            catch (SQLiteException error)
            {
                SqlErrorMessage = error.Message.ToString();
                SqlErrorNum = error.ErrorCode;
                SqlError = true;
            }
            finally
            {
                SqlCmd.Dispose();
                Connection.Close();
            }
        }

        /// <summary>
        /// Sends a SQL query that doesn't return any value
        /// It can also be used to create a table, but it's recomended to use CreateDb instead
        /// </summary>
        /// <param name="__Sql">SQL statement</param>
        public void SendQuery(string __Sql)
        {

            // Check if occurred any error
            if (GeneralError)
            {
                return;
            }

            SQLiteCommand SqlCmd = null;
            try
            {
                Connection.Open();
                SqlCmd = new SQLiteCommand(__Sql, Connection);
                SqlCmd.ExecuteNonQuery();
                SqlError = false;
            }
            catch (SQLiteException error)
            {
                SqlErrorMessage = error.Message.ToString();
                SqlErrorNum = error.ErrorCode;
                SqlError = true;
            }
            finally
            {
                SqlCmd.Dispose();
                Connection.Close();
            }
        }

        /// <summary>
        /// Gets the first result from the query
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <returns>Return the value inside a System.Object and must be converted</returns>
        public object Get(string __sql)
        {
            if (GeneralError)
            {
                return null;
            }

            object result = null;
            SQLiteCommand SqlCmd = null;

            try
            {
                Connection.Open();
                SqlCmd = new SQLiteCommand(__sql, Connection);
                result = SqlCmd.ExecuteScalar();
                SqlError = false;
            }
            catch (SQLiteException error)
            {
                SqlErrorMessage = error.Message.ToString();
                SqlErrorNum = error.ErrorCode;
                SqlError = true;
            }
            finally
            {
                SqlCmd.Dispose();
                Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Gets table values inside a dataset
        /// </summary>
        /// <param name="__sql">SQL statement</param>
        /// <param name="__table">Name of the table</param>
        /// <returns>All the returned value into a dataset</returns>
        public DataSet GetTable(string __sql, string __table)
        {
            // Check if occurred any error
            if (GeneralError)
            {
                return null;
            }

            DataSet data = null;
            SQLiteDataAdapter SqlAdapter = null;
            try
            {
                SqlAdapter = new SQLiteDataAdapter(__sql, Connection);
                SqlAdapter.Fill(data, __table);
                SqlError = false;
            }
            catch (SQLiteException error)
            {
                SqlErrorMessage = error.Message.ToString();
                SqlErrorNum = error.ErrorCode;
                SqlError = true;
            }
            finally
            {
                SqlAdapter.Dispose();
                Connection.Close();
            }

            return data;
        }
    }
}
