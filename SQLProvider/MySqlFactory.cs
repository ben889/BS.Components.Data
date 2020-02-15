using BS.Components.Data.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BS.Components.Data.SQLProvider
{
    public class MySqlFactory
    {
        private string connection = null;
        public MySqlFactory(string connection)
        {
            this.connection = connection;
        }

        public T GetModel<T>(object keyValue, string customKey, string customColumns) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            List<string> columns = MySqlEntityHelper.GetTableColumns(type, ColumnTypes.Read, customColumns);
            bool quote = false;
            PropertyInfo info = (customKey == null || customKey.Trim().Length == 0) ? MySqlEntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            if (info == null)
                return null;
            GetDbType(info.PropertyType, ref quote);
            StringBuilder sqlText = new StringBuilder();
            StringBuilder where = new StringBuilder();
            where.Append(" WHERE ");
            where.Append("`" + info.Name + "`");
            where.Append("=");
            if (quote)
            {
                where.Append("'");
                where.Append(keyValue);
                where.Append("'");
            }
            else
            {
                where.Append(keyValue);
            }
            GetBaseSelect(sqlText, columns, tableName, where.ToString(), 1);
            string str2 = type.Namespace + "." + type.Name;
            T local = default(T);
            MySqlDataReader reader = MySQLHelper.DataReader(this.connection, sqlText.ToString(), CommandType.Text, null);
            if (reader.Read())
            {
                local = MySqlEntityHelper.GetDataReaderObject<T>(type, str2, columns, reader);
            }
            reader.Close();
            return local;
        }

        public int Insert<T>(T model, ReturnTypes returnType) where T : class
        {
            ColumnTypes ColumnTypes = ColumnTypes.ReadInsert;
            if (returnType == ReturnTypes.Identity)
                ColumnTypes = ColumnTypes.Identity;
            return this._insert<T>(model, ColumnTypes, returnType);
        }
        public int Insert<T>(T model, MySqlConnection sqlconnection, MySqlCommand command, MySqlTransaction trans, ReturnTypes returnType) where T : class
        {
            ColumnTypes ColumnTypes = ColumnTypes.ReadInsert;
            if (returnType == ReturnTypes.Identity)
                ColumnTypes = ColumnTypes.Identity;
            return this._insert<T>(model, sqlconnection, command, trans, ColumnTypes, returnType);
        }
        public int Insert(string sql, MySqlConnection MySqlConnection, MySqlCommand command, MySqlTransaction trans, MySqlParameter[] parms, ReturnTypes returnType)
        {
            if (returnType == ReturnTypes.Identity)
            {
                object obj = MySQLHelper.ExecuteScalar(sql, MySqlConnection, command, trans, CommandType.Text, parms);
                return obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
            }
            else
            {
                return MySQLHelper.NonQuery(sql, MySqlConnection, command, trans, CommandType.Text, parms);
            }
        }

        public int Delete<T>(object keyValue, string customKey) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            bool quote = false;
            PropertyInfo info = (customKey == null || customKey.Trim().Length == 0) ? MySqlEntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            MySqlDbType dbType = GetDbType(info.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append(tableName);
            builder.Append(" WHERE ");
            builder.Append("`" + info.Name + "`");
            builder.Append("=");
            if (quote)
            {
                builder.Append("'");
                builder.Append(keyValue);
                builder.Append("'");
            }
            else
            {
                builder.Append(keyValue);
            }
            return MySQLHelper.NonQuery(this.connection, builder.ToString(), CommandType.Text, null);
        }

        public int Delete<T>(string where) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append("`" + tableName + "`");
            if (!string.IsNullOrEmpty(where))
            {
                builder.Append(" WHERE ");
                builder.Append(where);
            }
            return MySQLHelper.NonQuery(this.connection, builder.ToString(), CommandType.Text, null);
        }
        public int Delete<T>(string where, MySqlParameter[] parms) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append("`" + tableName + "`");
            if (!string.IsNullOrEmpty(where))
            {
                builder.Append(" WHERE ");
                builder.Append(where);
            }
            return MySQLHelper.NonQuery(this.connection, builder.ToString(), CommandType.Text, parms);
        }
        public int Delete<T>(MySqlConnection MySqlConnection, MySqlCommand command, MySqlTransaction trans, string where, MySqlParameter[] parms) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append("`" + tableName + "`");
            if (!string.IsNullOrEmpty(where))
            {
                builder.Append(" WHERE ");
                builder.Append(where);
            }
            return MySQLHelper.NonQuery(builder.ToString(), MySqlConnection, command, trans, CommandType.Text, parms);
        }

        public List<T> GetList<T>(int pageSize, string where, string orderBy, MySqlParameter[] parms, string customColumns) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            List<string> columns = MySqlEntityHelper.GetTableColumns(type, ColumnTypes.Read, customColumns);
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            StringBuilder sqlText = new StringBuilder();
            GetBaseSelect(sqlText, columns, tableName, where, pageSize);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.Append(" ORDER BY ");
                sqlText.Append(orderBy);
            }
            List<T> list2 = new List<T>();
            string str2 = type.Namespace + "." + type.Name;
            MySqlDataReader reader = MySQLHelper.DataReader(this.connection, sqlText.ToString(), CommandType.Text, parms);
            while (reader.Read())
            {
                list2.Add(MySqlEntityHelper.GetDataReaderObject<T>(type, str2, columns, reader));
            }
            reader.Close();
            return list2;
        }

        public int GetCount<T>(string where, MySqlParameter[] parms) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            String sqlText = "SELECT COUNT(1) FROM " + tableName;
            if (where != null && where.Trim().Length > 0)
                sqlText = sqlText + " WHERE " + where;
            object obj = MySQLHelper.ExecuteScalar(this.connection, sqlText.ToString(), CommandType.Text, parms);
            return obj != null ? Convert.ToInt32(obj) : 0;
        }

        public DataTable GetTable(string sqlText, CommandType commandType, MySqlParameter[] parms)
        {
            return MySQLHelper.DataSet(this.connection, sqlText, commandType, parms).Tables[0];
        }

        public DataTable GetTable<T>(int pageSize, string where, string orderBy, MySqlParameter[] parms, string customColumns) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            List<string> columns = MySqlEntityHelper.GetTableColumns(type, ColumnTypes.Read, customColumns);
            PropertyInfo tableIdentity = MySqlEntityHelper.GetTableIdentity(type);
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            StringBuilder sqlText = new StringBuilder();
            GetBaseSelect(sqlText, columns, tableName, where, pageSize);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlText.AppendFormat(" ORDER BY {0}", orderBy);
            }
            return MySQLHelper.DataSet(this.connection, sqlText.ToString(), CommandType.Text, parms).Tables[0];
        }

        public DataTable GetTablePager(int pageSize, int currentPage, string where, string orderBy, string columns, ref int records, MySqlParameter[] parms, string tableName, string Join)
        {
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            if (!string.IsNullOrEmpty(Join))
            {
                Join = " " + Join;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COUNT(1) FROM ");
            builder.Append(tableName);
            builder.Append(Join);
            builder.Append(where);
            builder.Append(";");
            builder.Append("SELECT ");
            if (columns != null && columns.Trim().Length > 0)
                builder.Append(columns);
            else
            {
                builder.Append(" * ");
            }
            builder.Append(" FROM ");
            builder.Append(tableName);
            builder.Append(Join);
            builder.Append(where);
            if (!string.IsNullOrEmpty(orderBy))
            {
                builder.Append(" ORDER BY " + orderBy);
            }
            int start = (currentPage - 1) * pageSize;
            builder.Append(" LIMIT " + start + "," + pageSize);
            DataTableCollection tables = MySQLHelper.DataSet(this.connection, builder.ToString(), CommandType.Text, parms).Tables;
            if (tables[0].Rows.Count > 0)
            {
                records = Convert.ToInt32(tables[0].Rows[0][0]);
            }
            return tables[1];
        }

        public int Update<T>(T model, string customColumns) where T : class
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            List<string> list = MySqlEntityHelper.GetTableColumns(type, ColumnTypes.ReadInsert, customColumns);
            bool quote = false;
            PropertyInfo tableIdentity = MySqlEntityHelper.GetTableIdentity(type);
            if (tableIdentity == null)
                return -1;
            GetDbType(tableIdentity.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE ");
            builder.Append("`" + tableName + "`");
            builder.Append(" SET ");
            //if (!tableIdentity.Name.Equals(list[0]))
            //{
            //    builder.Append(list[0]);
            //    builder.Append("=@");
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}
            int count = list.Count;
            for (index = 0; index < count; index++)
            {
                if (tableIdentity.Name.Equals(list[index]))
                    continue;
                builder.Append("`" + list[index] + "`");
                builder.Append("=@");
                builder.Append(list[index]);
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);

            builder.Append(" WHERE ");
            builder.Append(tableIdentity.Name);
            builder.Append("=");
            if (quote)
            {
                builder.Append("'");
                builder.Append(tableIdentity.GetValue(model, null));
                builder.Append("'");
            }
            else
            {
                builder.Append(tableIdentity.GetValue(model, null));
            }
            PropertyInfo property = null;
            MySqlParameter[] parms = new MySqlParameter[count];
            for (index = 0; index < count; index++)
            {
                property = type.GetProperty(list[index]);

                object dbType = null;
                //得到每一个属性的特性类集合
                IList<CustomAttributeData> lstAttr = property.GetCustomAttributesData();
                foreach (var oAttr in lstAttr)
                {
                    //得到特性类的所有参数
                    var lstAttrArgu = oAttr.NamedArguments;
                    foreach (var oAttrAru in lstAttrArgu)
                    {
                        //取每个特性类参数的键值对
                        //Console.WriteLine(oAttrAru.MemberInfo.Name + "=" + oAttrAru.TypedValue.Value);
                        if (oAttrAru.MemberInfo.Name.Trim().ToLower().Equals("dbtype"))
                        {
                            dbType = oAttrAru.TypedValue.Value;
                        }
                    }
                }

                parms[index] = new MySqlParameter("@" + list[index], GetDbType(property.PropertyType, dbType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            return MySQLHelper.NonQuery(connection, builder.ToString(), CommandType.Text, parms);
        }
        public int Update<T>(T model, MySqlConnection MySqlConnection, MySqlCommand command, MySqlTransaction trans, string customColumns) where T : class
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            List<string> list = MySqlEntityHelper.GetTableColumns(type, ColumnTypes.ReadInsert, customColumns);
            bool quote = false;
            PropertyInfo tableIdentity = MySqlEntityHelper.GetTableIdentity(type);
            if (tableIdentity == null)
                return -1;
            GetDbType(tableIdentity.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE ");
            builder.Append("`" + tableName + "`");
            builder.Append(" SET ");
            //if (!tableIdentity.Name.Equals(list[0]))
            //{
            //    builder.Append(list[0]);
            //    builder.Append("=@");
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}
            int count = list.Count;
            for (index = 0; index < count; index++)
            {
                if (tableIdentity.Name.Equals(list[index]))
                    continue;
                builder.Append("`" + list[index] + "`");
                builder.Append("=@");
                builder.Append(list[index]);
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);

            builder.Append(" WHERE ");
            builder.Append(tableIdentity.Name);
            builder.Append("=");
            if (quote)
            {
                builder.Append("'");
                builder.Append(tableIdentity.GetValue(model, null));
                builder.Append("'");
            }
            else
            {
                builder.Append(tableIdentity.GetValue(model, null));
            }
            PropertyInfo property = null;
            MySqlParameter[] parms = new MySqlParameter[count];
            for (index = 0; index < count; index++)
            {
                property = type.GetProperty(list[index]);
                parms[index] = new MySqlParameter("@" + list[index], GetDbType(property.PropertyType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            return MySQLHelper.NonQuery(builder.ToString(), MySqlConnection, command, trans, CommandType.Text, parms);
        }
        public int Update(string sql, MySqlConnection MySqlConnection, MySqlCommand command, MySqlTransaction trans, MySqlParameter[] parms)
        {
            return MySQLHelper.NonQuery(sql, MySqlConnection, command, trans, CommandType.Text, parms);
        }


        public int NonQuery(string sql)
        {
            return MySQLHelper.NonQuery(this.connection, sql, CommandType.Text, null);
        }
        public int NonQuery(string sql, MySqlParameter[] MySqlParameter)
        {
            return MySQLHelper.NonQuery(this.connection, sql, CommandType.Text, MySqlParameter);
        }

        public object ExecuteScalar(string sql, MySqlParameter[] MySqlParameter)
        {
            return MySQLHelper.ExecuteScalar(this.connection, sql, CommandType.Text, MySqlParameter);
        }

        public bool IsExist<T>(string where) where T : class
        {
            return GetCount<T>(where) > 0;
        }

        public int GetCount<T>(string where) where T : class
        {
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            //bool quote = false;
            //PropertyInfo tableIdentity = MySqlEntityHelper.GetTableIdentity(type);
            //GetDbType(tableIdentity.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COUNT(1) FROM ");
            builder.Append(tableName);
            if (where != null && where.Trim().Length > 0)
            {
                builder.Append(" WHERE ");
                builder.Append(where);
            }

            int num = 0;
            MySqlDataReader reader = MySQLHelper.DataReader(this.connection, builder.ToString(), CommandType.Text, null);
            if (reader.Read())
            {
                num = reader.GetInt32(0);
            }
            reader.Close();
            return num;
        }

        #region 私有方法
        public static MySqlDbType GetDbType(Type type, ref bool quote)
        {
            return GetDbType(type, "", ref quote);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbType"></param>
        /// <param name="quote">如果为真则为string</param>
        /// <returns></returns>
        private static MySqlDbType GetDbType(Type type, object dbType, ref bool quote)
        {
            if (dbType != null)
            {
                if (Enum.IsDefined(typeof(MySqlDbType), dbType))
                    return (MySqlDbType)Enum.ToObject(typeof(MySqlDbType), dbType);
            }
            quote = false;
            MySqlDbType varChar = MySqlDbType.VarChar;
            if (type.Equals(typeof(string)))
            {
                quote = true;
                return varChar;
            }
            if (type.Equals(typeof(int)))
            {
                return MySqlDbType.Int32;
            }
            if (type.Equals(typeof(bool)))
            {
                return MySqlDbType.Bit;
            }
            if (type.Equals(typeof(DateTime)))
            {
                quote = true;
                return MySqlDbType.DateTime;
            }
            if (type.Equals(typeof(decimal)))
            {
                return MySqlDbType.Decimal;
            }
            if (type.Equals(typeof(float)))
            {
                return MySqlDbType.Float;
            }
            if (type.Equals(typeof(double)))
            {
                return MySqlDbType.Float;
            }
            if (type.Equals(typeof(long)))
            {
                return MySqlDbType.Int64;
            }
            if (type.Equals(typeof(byte[])))
            {
                quote = true;
                varChar = MySqlDbType.Binary;
            }
            return varChar;
        }

        #region 添加
        private int _insert<T>(T model, ColumnTypes filter, ReturnTypes returnType)
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            List<string> list = MySqlEntityHelper.GetTableColumns(type, filter, null);
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append("`" + tableName + "`");
            builder.Append(" (");
            //if (returnType != ReturnTypes.Identity)
            //{
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}
            int count = list.Count;
            for (index = 0; index < count; index++)
            {

                builder.Append("`" + list[index] + "`");
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(") VALUES (");
            //if (returnType != ReturnTypes.Identity)
            //{
            //    builder.Append("@");
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}


            for (index = 0; index < count; index++)
            {
                builder.Append("@");
                builder.Append(list[index]);
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(");");
            bool quote = false;
            PropertyInfo property = null;
            MySqlParameter[] parms = new MySqlParameter[count];
            for (index = 0; index < count; index++)
            {

                property = type.GetProperty(list[index]);
                object dbType = null;
                //得到每一个属性的特性类集合
                IList<CustomAttributeData> lstAttr = property.GetCustomAttributesData();
                foreach (var oAttr in lstAttr)
                {
                    //得到特性类的所有参数
                    var lstAttrArgu = oAttr.NamedArguments;
                    foreach (var oAttrAru in lstAttrArgu)
                    {
                        //取每个特性类参数的键值对
                        //Console.WriteLine(oAttrAru.MemberInfo.Name + "=" + oAttrAru.TypedValue.Value);
                        if (oAttrAru.MemberInfo.Name.Trim().ToLower().Equals("dbtype"))
                        {
                            dbType = oAttrAru.TypedValue.Value;
                        }
                    }
                }


                //property = type.GetProperty(list[index]);
                parms[index] = new MySqlParameter("@" + list[index], GetDbType(property.PropertyType, dbType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            if (((returnType == ReturnTypes.EffectRow) || (returnType == ReturnTypes.Identity)) || (returnType == ReturnTypes.None))
            {
                if (returnType == ReturnTypes.Identity)
                {
                    builder.Append("SELECT LAST_INSERT_ID();");
                    object obj = MySQLHelper.ExecuteScalar(connection, builder.ToString(), CommandType.Text, parms);
                    return obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
                }
                else
                {
                    int result = MySQLHelper.NonQuery(connection, builder.ToString(), CommandType.Text, parms);
                    return result;
                }
            }
            return MySQLHelper.NonQuery(connection, builder.ToString(), CommandType.Text, parms);
        }

        private int _insert<T>(T model, MySqlConnection MySqlConnection, MySqlCommand command, MySqlTransaction trans, ColumnTypes filter, ReturnTypes returnType)
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = MySqlEntityHelper.GetTableName(type);
            List<string> list = MySqlEntityHelper.GetTableColumns(type, filter, null);
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append("`" + tableName + "`");
            builder.Append(" (");
            //if (returnType != ReturnTypes.Identity)
            //{
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}
            int count = list.Count;
            for (index = 0; index < count; index++)
            {

                builder.Append("`" + list[index] + "`");
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(") VALUES (");
            //if (returnType != ReturnTypes.Identity)
            //{
            //    builder.Append("@");
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}
            for (index = 0; index < count; index++)
            {
                builder.Append("@");
                builder.Append(list[index]);
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(");");
            bool quote = false;
            PropertyInfo property = null;
            MySqlParameter[] parms = new MySqlParameter[count];
            for (index = 0; index < count; index++)
            {
                property = type.GetProperty(list[index]);
                parms[index] = new MySqlParameter("@" + list[index], GetDbType(property.PropertyType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            if (((returnType == ReturnTypes.EffectRow) || (returnType == ReturnTypes.Identity)) || (returnType == ReturnTypes.None))
            {
                if (returnType == ReturnTypes.Identity)
                {
                    builder.Append("SELECT LAST_INSERT_ID();");
                    object obj = MySQLHelper.ExecuteScalar(builder.ToString(), MySqlConnection, command, trans, CommandType.Text, parms);
                    return obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
                }
                else
                {
                    int result = MySQLHelper.NonQuery(builder.ToString(), MySqlConnection, command, trans, CommandType.Text, parms);
                    return result;
                }
            }
            return MySQLHelper.NonQuery(builder.ToString(), MySqlConnection, command, trans, CommandType.Text, parms);
        }
        #endregion
        private static void GetBaseSelect(StringBuilder sqlText, List<string> columns, string tableName, string where, int pagesize)
        {
            sqlText.Append("SELECT ");

            if (columns != null && columns.Count > 0)
            {
                sqlText.Append("`" + columns[0] + "`");
                int count = columns.Count;
                for (int i = 1; i < count; i++)
                {
                    sqlText.Append(",");
                    sqlText.Append("`" + columns[i] + "`");
                }
            }
            else
            {
                sqlText.Append("*");
            }
            sqlText.Append(" FROM ");
            sqlText.Append(tableName);
            if (where != null && where.Trim().Length > 0)
            {
                sqlText.Append(where);
            }
            if (pagesize > 0)
            {
                sqlText.Append(" LIMIT 0," + pagesize);
            }
        }
        #endregion
    }
}
