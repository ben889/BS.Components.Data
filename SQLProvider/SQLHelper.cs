using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BS.Components.Data.SQLProvider
{
    public class SQLHelper
    {
        public static SqlDataReader DataReader(string connection, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            SqlConnection connection2 = new SqlConnection(connection);
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(connection2, null, sqlCommand, sqlText, commandType, parms);
            SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            sqlCommand.Parameters.Clear();
            sqlCommand.Dispose();
            return reader;
        }
        public static SqlDataReader DataReader(SqlConnection connection, SqlTransaction trans, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(connection, trans, sqlCommand, sqlText, commandType, parms);
            SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            sqlCommand.Parameters.Clear();
            sqlCommand.Dispose();
            return reader;
        }
        //唐
        //public static object ExecuteScalar(string connection, string sqlText, CommandType commandType, DbParameter[] parms)
        //{
        //    SqlConnection connection2 = new SqlConnection(connection);
        //    SqlCommand sqlCommand = new SqlCommand();
        //    PrepareCommand(connection2, null, sqlCommand, sqlText, commandType, parms);
        //    object obj = sqlCommand.ExecuteScalar();
        //    sqlCommand.Parameters.Clear();
        //    sqlCommand.Dispose();
        //    return obj;
        //}

        public static object ExecuteScalar(string connection, string sqlText, CommandType commandType, DbParameter[] parameters)
        {
            object result = null;
            using (SqlConnection connection2 = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(sqlText, connection2))
                {
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection2.Open();
                    result = command.ExecuteScalar();
                }
            }
            return result;
        }

        public static object ExecuteScalar(string sqlText, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans
            , CommandType commandType, DbParameter[] parameters)
        {
            object result = null;
            //command.CommandText = sqlText;
            //command.CommandType = commandType;
            //if (trans != null)
            //{
            //    command.Transaction = trans;
            //}
            //if (parameters != null)
            //{
            //    foreach (SqlParameter parameter in parameters)
            //    {
            //        command.Parameters.Add(parameter);
            //    }
            //}
            PrepareCommand(sqlconnection, trans, command, sqlText, commandType, parameters);
            result = command.ExecuteScalar();


            return result;
        }

        public static System.Data.DataSet DataSet(string connection, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            using (SqlConnection connection2 = new SqlConnection(connection))
            {
                SqlCommand sqlCommand = new SqlCommand();
                PrepareCommand(connection2, null, sqlCommand, sqlText, commandType, parms);
                System.Data.DataSet dataSet = new System.Data.DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlText, connection2)
                {
                    SelectCommand = sqlCommand
                };
                adapter.SelectCommand.CommandTimeout = 60;
                adapter.Fill(dataSet);
                adapter.Dispose();
                connection2.Close();
                return dataSet;
            }
        }

        public static DbParameter MakeParam(string paramName, SqlDbType type, int size)
        {
            if (size > 0)
            {
                return new SqlParameter(paramName, type, size);
            }
            return new SqlParameter(paramName, type);
        }

        //public static int NonQuery(SqlConnection connection, string sqlText, CommandType commandType, params DbParameter[] parms)
        //{
        //    SqlCommand sqlCommand = new SqlCommand();
        //    PrepareCommand(connection, null, sqlCommand, sqlText, commandType, parms);
        //    int num = sqlCommand.ExecuteNonQuery();
        //    sqlCommand.Parameters.Clear();
        //    sqlCommand.Dispose();
        //    return num;
        //}

        public static int NonQuery(string connection, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            int num = 0;
            using (SqlConnection connection2 = new SqlConnection(connection))
            {
                SqlCommand sqlCommand = new SqlCommand();
                PrepareCommand(connection2, null, sqlCommand, sqlText, commandType, parms);
                num = sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                sqlCommand.Dispose();
            }
            return num;
        }

        public static int NonQuery(string sqlText, SqlConnection sqlconnection, SqlCommand sqlCommand, SqlTransaction trans, CommandType commandType, DbParameter[] parms)
        {
            int num = 0;
            PrepareCommand(sqlconnection, trans, sqlCommand, sqlText, commandType, parms);
            num = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return num;
        }

        private static void PrepareCommand(SqlConnection connection, SqlTransaction trans, SqlCommand sqlCommand, string sqlText, CommandType commandType, params DbParameter[] parms)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            sqlCommand.Connection = connection;
            sqlCommand.CommandText = sqlText;
            sqlCommand.CommandType = commandType;
            sqlCommand.CommandTimeout = 60;
            if (trans != null)
            {
                sqlCommand.Transaction = trans;
            }
            if (parms != null)
            {
                foreach (DbParameter parameter in parms)
                {
                    sqlCommand.Parameters.Add((SqlParameter)parameter);
                }
            }
        }
    }
}
