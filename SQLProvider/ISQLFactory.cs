using BS.Components.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace BS.Components.Data.SQLProvider
{
    public interface ISQLFactory
    {
        /// <summary>
        /// 根据字段查实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue">key字段值</param>
        /// <param name="customKey">key字段名(可为空)</param>
        /// <param name="customColumns">要查询的字段;*为全部;取指定字段(aaa,bbb);空为取全部</param>
        /// <returns></returns>
        T GetModel<T>(object keyValue, string customKey, string customColumns) where T : class;
        int Insert<T>(T model, ReturnTypes returnType) where T : class;
        int Insert<T>(T model, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, ReturnTypes returnType) where T : class;
        int Insert(string sql, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, DbParameter[] parms, ReturnTypes returnType);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">实体类</param>
        /// <param name="customColumns">为空则改全部字段;如aaa='aaa',bbb='bbb'则改指定字段</param>
        int Update<T>(T model, string customColumns) where T : class;
        int Update<T>(T model, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, string customColumns) where T : class;
        int Update(string sql, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, DbParameter[] parms);

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue"></param>
        /// <param name="customKey">字段名;如为空则按索引删除</param>
        /// <returns></returns>
        int Delete<T>(object keyValue, string customKey) where T : class;
        int Delete<T>(string where) where T : class;
        int Delete<T>(SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, string where) where T : class;

        List<T> GetList<T>(int pageSize, string where, string orderBy, DbParameter[] parms, string customColumns) where T : class;
        int GetCount<T>(string where, DbParameter[] parms) where T : class;

        DataTable GetTable(string sqlText, CommandType commandType, DbParameter[] parms);
        DataTable GetTable<T>(int pageSize, string where, string orderBy, DbParameter[] parms, string customColumns) where T : class;

        DataTable GetTablePager<T>(int pageSize, int currentPage, string where, string orderBy, string customColumns, ref int records, DbParameter[] parms) where T : class;
        DataTable GetTablePager(int pageSize, int currentPage, string where, string orderBy, string columns, ref int records, DbParameter[] parms, string tableName, string Join);
        DataTable GetTablePager(int pageSize, int currentPage, string where, string orderBy, string columns, ref int records, DbParameter[] parms, string tableName);

        int NonQuery(string sql);
        int NonQuery(string sql, DbParameter[] dbparameter);
        object ExecuteScalar(string sql, DbParameter[] dbparameter);
        bool IsExist<T>(string where) where T : class;
        int GetCount<T>(string where) where T : class;
    }
}
