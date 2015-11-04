using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Web;

namespace BS.Components.Data.SQLProvider
{
    public abstract class AccessHelper
    {
        public static readonly string connectionString = ("provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + HttpContext.Current.Request.PhysicalApplicationPath + ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);

        protected AccessHelper()
        {
        }

        public static OleDbDataReader DataReader(string sqlText, OleDbParameter[] parms)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand sqlCommand = new OleDbCommand();
            PrepareCommand(conn, sqlCommand, sqlText, parms);
            OleDbDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            sqlCommand.Dispose();
            return reader;
        }

        public static OleDbDataReader DataReader(OleDbConnection conn, string sqlText, OleDbParameter[] parms)
        {
            OleDbCommand sqlCommand = new OleDbCommand();
            PrepareCommand(conn, sqlCommand, sqlText, parms);
            OleDbDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            sqlCommand.Dispose();
            return reader;
        }
        public static object ExecuteScalar(string sqlText, CommandType commandType, OleDbParameter[] parms)
        {
            object obj;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand sqlCommand = new OleDbCommand();
                PrepareCommand(connection, sqlCommand, sqlText, parms);
                obj = sqlCommand.ExecuteScalar();
                sqlCommand.Parameters.Clear();
                sqlCommand.Dispose();
            }
            return obj;
        }
        public static System.Data.DataSet DataSet(string sqlText, OleDbParameter[] parms)
        {
            System.Data.DataSet dataSet = new System.Data.DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlText, connectionString)
            {
                SelectCommand = { CommandType = CommandType.Text }
            };
            if (parms != null)
            {
                foreach (OleDbParameter parameter in parms)
                {
                    adapter.SelectCommand.Parameters.Add(parameter);
                }
            }
            adapter.Fill(dataSet, "DataList");
            adapter.SelectCommand.Parameters.Clear();
            adapter.Dispose();
            return dataSet;
        }

        public static OleDbConnection GetSqlConnection()
        {
            return new OleDbConnection(connectionString);
        }

        public static OleDbConnection GetSqlConnection(string file)
        {
            return new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + HttpContext.Current.Request.PhysicalApplicationPath + file);
        }

        public static int NonQuery(string sqlText, OleDbParameter[] parms)
        {
            int num = 0;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand sqlCommand = new OleDbCommand();
                PrepareCommand(connection, sqlCommand, sqlText, parms);
                num = sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                sqlCommand.Dispose();
            }
            return num;
        }

        public static int NonQuery(OleDbConnection conn, string sqlText, OleDbParameter[] parms)
        {
            OleDbCommand sqlCommand = new OleDbCommand();
            PrepareCommand(conn, sqlCommand, sqlText, parms);
            int num = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            sqlCommand.Dispose();
            return num;
        }

        private static void PrepareCommand(OleDbConnection conn, OleDbCommand sqlCommand, string sqlText, OleDbParameter[] parms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            sqlCommand.Connection = conn;
            sqlCommand.CommandText = sqlText;
            sqlCommand.CommandType = CommandType.Text;
            if (parms != null)
            {
                foreach (OleDbParameter parameter in parms)
                {
                    sqlCommand.Parameters.Add(parameter);
                }
            }
        }
    }
}
