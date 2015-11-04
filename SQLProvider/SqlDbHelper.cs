using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace BS.Components.Data.SQLProvider
{
    /// <summary>
    /// 数据库访问通用类--2011-01-24 by rqli
    /// </summary>
    public class SqlDbHelper
    {

        /// <summary>
        /// 执行一个查询，并返回查询结果
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL文本命令</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string Connection, string Sql)
        {
            return ExecuteDataTable(Connection, Sql, CommandType.Text, null);
        }
        /// <summary>
        /// 执行一个查询，并返回查询结果
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string Connection, string Sql, CommandType CommandType)
        {
            return ExecuteDataTable(Connection, Sql, CommandType, null);
        }

        /// <summary>
        /// 执行一个查询，并返回查询结果
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="Parameter">Transact-SQL语句或存储过程的参数数组</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string Connection, string Sql, CommandType CommandType, SqlParameter[] Parameters)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                using (SqlCommand command = new SqlCommand(Sql, connection))
                {
                    command.CommandType = CommandType;
                    if (Parameters != null)
                    {
                        foreach (SqlParameter parameter in Parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(data);
                    return data;
                }
            }
        }
        /// <summary>
        /// 返回一个SqlDataReader对象的实例
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL文本命令</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string Connection, string Sql)
        {
            return ExecuteReader(Connection, Sql, CommandType.Text, null);
        }
        /// <summary>
        /// 返回一个SqlDataReader对象的实例
        /// </summary>
        ///  <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string Connection, string Sql, CommandType CommandType)
        {
            return ExecuteReader(Connection, Sql, CommandType, null);
        }

        /// <summary>
        /// 返回一个SqlDataReader对象的实例
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="Parameters">Transact-SQL语句或存储过程的参数数组</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string Connection, string Sql, CommandType CommandType, SqlParameter[] Parameters)
        {
            SqlConnection connection = new SqlConnection(Connection);

            SqlCommand command = new SqlCommand(Sql, connection);

            command.CommandType = CommandType;
            if (Parameters != null)
            {
                foreach (SqlParameter parameter in Parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            connection.Open();
            SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
            command.Dispose();
            return dr;
        }
        /// <summary>
        /// 执行一个查询，返回查询结果集的第一行第一列，忽略其他行和列
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL文本命令</param>
        /// <returns></returns>
        public static object ExecuteScalar(string Connection, string Sql)
        {
            return ExecuteScalar(Connection, Sql, CommandType.Text, null);
        }
        /// <summary>
        /// 执行一个查询，返回查询结果集的第一行第一列，忽略其他行和列
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns></returns>
        public static object ExecuteScalar(string Connection, string Sql, CommandType CommandType)
        {
            return ExecuteScalar(Connection, Sql, CommandType, null);
        }
        /// <summary>
        /// 执行一个查询，返回查询结果集的第一行第一列，忽略其他行和列
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="Parameters">Transact-SQL语句或存储过程的参数数组</param>
        /// <returns></returns>
        public static object ExecuteScalar(string Connection, string Sql, CommandType CommandType, SqlParameter[] Parameters)
        {
            object result = null;
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                using (SqlCommand command = new SqlCommand(Sql, connection))
                {
                    command.CommandType = CommandType;
                    if (Parameters != null)
                    {
                        foreach (SqlParameter parameter in Parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();
                    result = command.ExecuteScalar();
                }
            }
            return result;
        }
        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="Sql">要执行查询的SQL文本命令</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string Connection, string Sql)
        {
            return ExecuteNonQuery(Connection, Sql, CommandType.Text, null);
        }
        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string Connection, string Sql, CommandType CommandType)
        {
            return ExecuteNonQuery(Connection, Sql, CommandType, null);
        }
        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="Connection">连接字符串</param>
        /// <param name="Sql">要执行查询的SQL语句</param>
        /// <param name="CommandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="Parameters">Transact-SQL语句或存储过程的参数数组</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string Connection, string Sql, CommandType CommandType, SqlParameter[] Parameters)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                using (SqlCommand command = new SqlCommand(Sql, connection))
                {
                    command.CommandType = CommandType;
                    if (Parameters != null)
                    {
                        foreach (SqlParameter parameter in Parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();
                    count = command.ExecuteNonQuery();
                }
            }
            return count;//执行增删改操作后受影响的行数
        }
    }
}
