using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace BS.Components.Data.SQLProvider
{
    /// <summary>
    ///  MySql数据库访问通用类--2019-5-1 by rqli
    /// </summary>
    public class MySQLHelper
    {
        public static MySqlDataReader DataReader(string connection, string sqlText, CommandType commandType, MySqlParameter[] parms)
        {
            MySqlConnection connection2 = new MySqlConnection(connection);
            MySqlCommand MySqlCommand = new MySqlCommand();
            PrepareCommand(connection2, null, MySqlCommand, sqlText, commandType, parms);
            MySqlDataReader reader = MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            MySqlCommand.Parameters.Clear();
            MySqlCommand.Dispose();
            return reader;
        }
        public static MySqlDataReader DataReader(MySqlConnection connection, MySqlTransaction trans, string sqlText, CommandType commandType, MySqlParameter[] parms)
        {
            MySqlCommand MySqlCommand = new MySqlCommand();
            PrepareCommand(connection, trans, MySqlCommand, sqlText, commandType, parms);
            MySqlDataReader reader = MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            MySqlCommand.Parameters.Clear();
            MySqlCommand.Dispose();
            return reader;
        }
        //唐
        //public static object ExecuteScalar(string connection, string sqlText, CommandType commandType, MySqlParameter[] parms)
        //{
        //    MySqlConnection connection2 = new MySqlConnection(connection);
        //    MySqlCommand MySqlCommand = new MySqlCommand();
        //    PrepareCommand(connection2, null, MySqlCommand, sqlText, commandType, parms);
        //    object obj = MySqlCommand.ExecuteScalar();
        //    MySqlCommand.Parameters.Clear();
        //    MySqlCommand.Dispose();
        //    return obj;
        //}

        public static object ExecuteScalar(string connection, string sqlText, CommandType commandType, MySqlParameter[] parameters)
        {
            object result = null;
            using (MySqlConnection connection2 = new MySqlConnection(connection))
            {
                using (MySqlCommand command = new MySqlCommand(sqlText, connection2))
                {
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (MySqlParameter parameter in parameters)
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

        public static object ExecuteScalar(string sqlText, MySqlConnection MySqlConnection, MySqlCommand command, MySqlTransaction trans
            , CommandType commandType, MySqlParameter[] parameters)
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
            //    foreach (MySqlParameter parameter in parameters)
            //    {
            //        command.Parameters.Add(parameter);
            //    }
            //}
            PrepareCommand(MySqlConnection, trans, command, sqlText, commandType, parameters);
            result = command.ExecuteScalar();


            return result;
        }

        public static System.Data.DataSet DataSet(string connection, string sqlText, CommandType commandType, MySqlParameter[] parms)
        {
            using (MySqlConnection connection2 = new MySqlConnection(connection))
            {
                MySqlCommand MySqlCommand = new MySqlCommand();
                PrepareCommand(connection2, null, MySqlCommand, sqlText, commandType, parms);
                System.Data.DataSet dataSet = new System.Data.DataSet();
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlText, connection2)
                {
                    SelectCommand = MySqlCommand
                };
                adapter.SelectCommand.CommandTimeout = 60;
                adapter.Fill(dataSet);
                adapter.Dispose();
                connection2.Close();
                return dataSet;
            }
        }

        public static MySqlParameter MakeParam(string paramName, MySqlDbType type, int size)
        {
            if (size > 0)
            {
                return new MySqlParameter(paramName, type, size);
            }
            return new MySqlParameter(paramName, type);
        }

        //public static int NonQuery(MySqlConnection connection, string sqlText, CommandType commandType, params MySqlParameter[] parms)
        //{
        //    MySqlCommand MySqlCommand = new MySqlCommand();
        //    PrepareCommand(connection, null, MySqlCommand, sqlText, commandType, parms);
        //    int num = MySqlCommand.ExecuteNonQuery();
        //    MySqlCommand.Parameters.Clear();
        //    MySqlCommand.Dispose();
        //    return num;
        //}

        public static int NonQuery(string connection, string sqlText, CommandType commandType, MySqlParameter[] parms)
        {
            int num = 0;
            using (MySqlConnection connection2 = new MySqlConnection(connection))
            {
                MySqlCommand MySqlCommand = new MySqlCommand();
                PrepareCommand(connection2, null, MySqlCommand, sqlText, commandType, parms);
                num = MySqlCommand.ExecuteNonQuery();
                MySqlCommand.Parameters.Clear();
                MySqlCommand.Dispose();
            }
            return num;
        }

        public static int NonQuery(string sqlText, MySqlConnection MySqlConnection, MySqlCommand MySqlCommand, MySqlTransaction trans, CommandType commandType, MySqlParameter[] parms)
        {
            int num = 0;
            PrepareCommand(MySqlConnection, trans, MySqlCommand, sqlText, commandType, parms);
            num = MySqlCommand.ExecuteNonQuery();
            MySqlCommand.Parameters.Clear();
            return num;
        }

        private static void PrepareCommand(MySqlConnection connection, MySqlTransaction trans, MySqlCommand MySqlCommand, string sqlText, CommandType commandType, params MySqlParameter[] parms)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            MySqlCommand.Connection = connection;
            MySqlCommand.CommandText = sqlText;
            MySqlCommand.CommandType = commandType;
            MySqlCommand.CommandTimeout = 60;
            if (trans != null)
            {
                MySqlCommand.Transaction = trans;
            }
            if (parms != null)
            {
                foreach (MySqlParameter parameter in parms)
                {
                    MySqlCommand.Parameters.Add((MySqlParameter)parameter);
                }
            }
        }
    }
}
