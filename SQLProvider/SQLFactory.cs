using BS.Components.Data.Entity;
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
    public class SQLFactory : ISQLFactory
    {

        private string connection = null;

        public SQLFactory(string connection)
        {

            this.connection = connection;
        }

        public T GetModel<T>(object keyValue, string customKey, string customColumns) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Read, customColumns);
            bool quote = false;
            PropertyInfo info = (customKey == null || customKey.Trim().Length == 0) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            if (info == null)
                return null;
            GetDbType(info.PropertyType, ref quote);
            StringBuilder sqlText = new StringBuilder();
            GetBaseSelect(sqlText, columns, tableName, "", 1);
            sqlText.Append(" WHERE ");
            sqlText.Append(info.Name);
            sqlText.Append("=");
            if (quote)
            {
                sqlText.Append("'");
                sqlText.Append(keyValue);
                sqlText.Append("'");
            }
            else
            {
                sqlText.Append(keyValue);
            }
            string str2 = type.Namespace + "." + type.Name;
            T local = default(T);
            SqlDataReader reader = SQLHelper.DataReader(this.connection, sqlText.ToString(), CommandType.Text, null);
            if (reader.Read())
            {
                local = EntityHelper.GetDataReaderObject<T>(type, str2, columns, reader);
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
        public int Insert<T>(T model, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, ReturnTypes returnType) where T : class
        {
            ColumnTypes ColumnTypes = ColumnTypes.ReadInsert;
            if (returnType == ReturnTypes.Identity)
                ColumnTypes = ColumnTypes.Identity;
            return this._insert<T>(model, sqlconnection, command, trans, ColumnTypes, returnType);
        }

        public int Insert(string sql, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, DbParameter[] parms, ReturnTypes returnType)
        {
            if (returnType == ReturnTypes.Identity)
            {
                object obj = SQLHelper.ExecuteScalar(sql, sqlconnection, command, trans, CommandType.Text, parms);
                return obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
            }
            else
            {
                return SQLHelper.NonQuery(sql, sqlconnection, command, trans, CommandType.Text, parms);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue"></param>
        /// <param name="customKey">字段名;如为空则按索引删除</param>
        /// <returns></returns>
        public int Delete<T>(object keyValue, string customKey) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            bool quote = false;
            PropertyInfo info = (customKey == null || customKey.Trim().Length == 0) ? EntityHelper.GetTableIdentity(type) : type.GetProperty(customKey);
            SqlDbType dbType = GetDbType(info.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append(tableName);
            builder.Append(" WHERE ");
            builder.Append(info.Name);
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
            return SQLHelper.NonQuery(this.connection, builder.ToString(), CommandType.Text, null);
        }

        public int Delete<T>(string where) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append(tableName);
            if (!string.IsNullOrEmpty(where))
            {
                builder.Append(" WHERE ");
                builder.Append(where);
            }
            return SQLHelper.NonQuery(this.connection, builder.ToString(), CommandType.Text, null);
        }
        public int Delete<T>(SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, string where) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append(tableName);
            if (!string.IsNullOrEmpty(where))
            {
                builder.Append(" WHERE ");
                builder.Append(where);
            }
            return SQLHelper.NonQuery(builder.ToString(), sqlconnection, command, trans, CommandType.Text, null);
        }

        public List<T> GetList<T>(int pageSize, string where, string orderBy, DbParameter[] parms, string customColumns) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Read, customColumns);
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
            SqlDataReader reader = SQLHelper.DataReader(this.connection, sqlText.ToString(), CommandType.Text, parms);
            while (reader.Read())
            {
                list2.Add(EntityHelper.GetDataReaderObject<T>(type, str2, columns, reader));
            }
            reader.Close();
            return list2;
        }

        public int GetCount<T>(string where, DbParameter[] parms) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            String sqlText = "SELECT COUNT(1) FROM " + tableName;
            if (where != null && where.Trim().Length > 0)
                sqlText = sqlText + " WHERE " + where;
            object obj = SQLHelper.ExecuteScalar(this.connection, sqlText.ToString(), CommandType.Text, parms);
            return obj != null ? Convert.ToInt32(obj) : 0;
        }

        public DataTable GetTable(string sqlText, CommandType commandType, DbParameter[] parms)
        {
            return SQLHelper.DataSet(this.connection, sqlText, commandType, parms).Tables[0];
        }

        public DataTable GetTable<T>(int pageSize, string where, string orderBy, DbParameter[] parms, string customColumns) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Read, customColumns);
            PropertyInfo tableIdentity = EntityHelper.GetTableIdentity(type);
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
            return SQLHelper.DataSet(this.connection, sqlText.ToString(), CommandType.Text, parms).Tables[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">实体类</param>
        /// <param name="customColumns">为空则改全部字段;如aaa='aaa',bbb='bbb'则改指定字段</param>
        public int Update<T>(T model, string customColumns) where T : class
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            List<string> list = EntityHelper.GetTableColumns(type, ColumnTypes.ReadInsert, customColumns);
            bool quote = false;
            PropertyInfo tableIdentity = EntityHelper.GetTableIdentity(type);
            if (tableIdentity == null)
                return -1;
            GetDbType(tableIdentity.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE ");
            builder.Append(tableName);
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
                builder.Append(list[index]);
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
            SqlParameter[] parms = new SqlParameter[count];
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

                parms[index] = new SqlParameter("@" + list[index], GetDbType(property.PropertyType, dbType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            return SQLHelper.NonQuery(connection, builder.ToString(), CommandType.Text, parms);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="sqlconnection"></param>
        /// <param name="command"></param>
        /// <param name="trans"></param>
        /// <param name="customColumns">为空则改全部字段;如aaa='aaa',bbb='bbb'则改指定字段</param>
        /// <returns></returns>
        public int Update<T>(T model, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, string customColumns) where T : class
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            List<string> list = EntityHelper.GetTableColumns(type, ColumnTypes.ReadInsert, customColumns);
            bool quote = false;
            PropertyInfo tableIdentity = EntityHelper.GetTableIdentity(type);
            if (tableIdentity == null)
                return -1;
            GetDbType(tableIdentity.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE ");
            builder.Append(tableName);
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
                builder.Append(list[index]);
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
            SqlParameter[] parms = new SqlParameter[count];
            for (index = 0; index < count; index++)
            {
                property = type.GetProperty(list[index]);
                parms[index] = new SqlParameter("@" + list[index], GetDbType(property.PropertyType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            return SQLHelper.NonQuery(builder.ToString(), sqlconnection, command, trans, CommandType.Text, parms);
        }
        public int Update(string sql, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, DbParameter[] parms)
        {
            return SQLHelper.NonQuery(sql, sqlconnection, command, trans, CommandType.Text, parms);
        }


        //public int Update<T>(T model, Dictionary<string, object> dic)
        //{
        //    Type type = typeof(T);
        //    string tableName = EntityHelper.GetTableName(type);
        //    bool quote = false;
        //    PropertyInfo tableIdentity = EntityHelper.GetTableIdentity(type);
        //    if (tableIdentity == null)
        //        return -1;
        //    GetDbType(tableIdentity.PropertyType, ref quote);
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append("UPDATE ");
        //    builder.Append(tableName);
        //    builder.Append(" SET ");
        //    int length = dic.Count;
        //    SqlParameter[] parms = new SqlParameter[length];
        //    int i = 0;
        //    foreach (var item in dic)
        //    {
        //        //Console.WriteLine("key:{0} value:{1}", item.Key, item.Value);
        //        string name = item.Key;
        //        SqlDbType dbType = GetDbType(type.GetProperty(name).PropertyType, ref quote);
        //        builder.Append(name);
        //        builder.Append("=@");
        //        builder.Append(name);
        //        builder.Append(",");
        //        parms[i] = new SqlParameter("@" + name, dbType);
        //        parms[i].Value = item.Value;
        //        i++;
        //    }
        //    builder.Remove(builder.Length - 1, 1);

        //    builder.Append(" WHERE ");
        //    builder.Append(tableIdentity.Name);
        //    builder.Append("=");
        //    if (quote)
        //    {
        //        builder.Append("'");
        //        builder.Append(tableIdentity.GetValue(model, null));
        //        builder.Append("'");
        //    }
        //    else
        //    {
        //        builder.Append(tableIdentity.GetValue(model, null));
        //    }
        //    string sql = builder.Remove(builder.Length - 1, 1).ToString();
        //    return SQLHelper.NonQuery(this.connection, builder.ToString(), CommandType.Text, parms);
        //}


        public DataTable GetTablePager<T>(int pageSize, int currentPage, string where, string orderBy, string customColumns, ref int records, DbParameter[] parms) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            if (customColumns == null)
            {
                customColumns = "*";
            }
            List<string> list = EntityHelper.GetTableColumns(type, ColumnTypes.Read, customColumns);
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COUNT(1) FROM ");
            builder.Append(tableName);
            builder.Append(where);
            builder.Append(";");
            if (!string.IsNullOrEmpty(orderBy))
            {
                builder.Append("Select * from (select (ROW_NUMBER() OVER(ORDER BY " + orderBy + ")) AS rownum,");
            }
            else
            {
                builder.Append("Select * from (select (ROW_NUMBER() OVER(ORDER BY getdate())) AS rownum,");
            }
            if (list.Count > 0)
            {
                builder.Append(list[0]);
                int count = list.Count;
                for (int i = 1; i < count; i++)
                {
                    builder.Append(",");
                    builder.Append(list[i]);
                }
            }
            else
            {
                builder.Append("*");
            }
            builder.Append(" from ");
            builder.Append(tableName);
            builder.Append(where);
            builder.Append(string.Concat(new object[] { ") tmp where rownum between ", (currentPage - 1) * pageSize + 1, " and ", currentPage * pageSize }));
            DataTableCollection tables = SQLHelper.DataSet(this.connection, builder.ToString(), CommandType.Text, parms).Tables;
            if (tables[0].Rows.Count > 0)
            {
                records = Convert.ToInt32(tables[0].Rows[0][0]);
            }
            return tables[1];
        }


        public DataTable GetTablePager(int pageSize, int currentPage, string where, string orderBy, string columns, ref int records, DbParameter[] parms, string tableName, string Join)
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
            if (!string.IsNullOrEmpty(orderBy))
            {
                builder.Append("Select * from (select (ROW_NUMBER() OVER(ORDER BY " + orderBy + ")) AS rownum,");
            }
            else
            {
                builder.Append("Select * from (select (ROW_NUMBER() OVER(ORDER BY getdate())) AS rownum,");
            }
            if (columns != null && columns.Trim().Length > 0)
                builder.Append(columns);
            else
            {
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append(" from ");
            builder.Append(tableName);
            builder.Append(Join);
            builder.Append(where);
            builder.Append(string.Concat(new object[] { ") tmp where rownum between ", (currentPage - 1) * pageSize + 1, " and ", currentPage * pageSize }));
            builder.Append(";");
            DataTableCollection tables = SQLHelper.DataSet(this.connection, builder.ToString(), CommandType.Text, parms).Tables;
            if (tables[0].Rows.Count > 0)
            {
                records = Convert.ToInt32(tables[0].Rows[0][0]);
            }
            return tables[1];
        }
        public DataTable GetTablePager(int pageSize, int currentPage, string where, string orderBy, string columns, ref int records, DbParameter[] parms, string tableName)
        {
            return GetTablePager(pageSize, currentPage, where, orderBy, columns, ref records, parms, tableName, null);
        }

        public int NonQuery(string sql)
        {
            return SQLHelper.NonQuery(this.connection, sql, CommandType.Text, null);
        }
        public int NonQuery(string sql, DbParameter[] dbparameter)
        {
            return SQLHelper.NonQuery(this.connection, sql, CommandType.Text, dbparameter);
        }

        public object ExecuteScalar(string sql, DbParameter[] dbparameter)
        {
            return SQLHelper.ExecuteScalar(this.connection, sql, CommandType.Text, dbparameter);
        }

        public bool IsExist<T>(string where) where T : class
        {
            return GetCount<T>(where) > 0;
            //Type type = typeof(T);
            //string tableName = EntityHelper.GetTableName(type);
            //bool quote = false;
            //PropertyInfo tableIdentity = EntityHelper.GetTableIdentity(type);
            //GetDbType(tableIdentity.PropertyType, ref quote);
            //StringBuilder builder = new StringBuilder();
            //builder.Append("SELECT COUNT(*) FROM ");
            //builder.Append(tableName);
            //builder.Append(" WHERE ");
            //builder.Append(where);
            //int num = 0;
            //SqlDataReader reader = SQLHelper.DataReader(this.connection, builder.ToString(), CommandType.Text, null);
            //if (reader.Read())
            //{
            //    num = reader.GetInt32(0);
            //}
            //reader.Close();
            //return (num > 0);
        }

        public int GetCount<T>(string where) where T : class
        {
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            bool quote = false;
            PropertyInfo tableIdentity = EntityHelper.GetTableIdentity(type);
            GetDbType(tableIdentity.PropertyType, ref quote);
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COUNT(*) FROM ");
            builder.Append(tableName);
            builder.Append(" WHERE ");
            builder.Append(where);
            int num = 0;
            SqlDataReader reader = SQLHelper.DataReader(this.connection, builder.ToString(), CommandType.Text, null);
            if (reader.Read())
            {
                num = reader.GetInt32(0);
            }
            reader.Close();
            return num;
        }

        #region 私有方法
        private static SqlDbType GetDbType(Type type, ref bool quote)
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
        private static SqlDbType GetDbType(Type type, object dbType, ref bool quote)
        {
            if (dbType != null)
            {
                if (Enum.IsDefined(typeof(SqlDbType), dbType))
                    return (SqlDbType)Enum.ToObject(typeof(SqlDbType), dbType);
            }
            quote = false;
            SqlDbType varChar = SqlDbType.VarChar;
            if (type.Equals(typeof(string)))
            {
                quote = true;
                return varChar;
            }
            if (type.Equals(typeof(int)))
            {
                return SqlDbType.Int;
            }
            if (type.Equals(typeof(bool)))
            {
                return SqlDbType.Bit;
            }
            if (type.Equals(typeof(DateTime)))
            {
                quote = true;
                return SqlDbType.DateTime;
            }
            if (type.Equals(typeof(decimal)))
            {
                return SqlDbType.Decimal;
            }
            if (type.Equals(typeof(float)))
            {
                return SqlDbType.Float;
            }
            if (type.Equals(typeof(double)))
            {
                return SqlDbType.Float;
            }
            if (type.Equals(typeof(long)))
            {
                return SqlDbType.BigInt;
            }
            if (type.Equals(typeof(byte[])))
            {
                quote = true;
                varChar = SqlDbType.Binary;
            }
            return varChar;
        }
        /// <summary>
        /// 2017-3-3新增
        /// </summary>
        /// <param name="dbTypeName"></param>
        /// <param name="raise"></param>
        /// <returns></returns>
        private static SqlDbType ParseDbType(string dbTypeName, bool raise = true)
        {
            switch (dbTypeName.ToLower())
            {
                case "bigint": return SqlDbType.BigInt;
                case "binary": return SqlDbType.Binary;
                case "bit": return SqlDbType.Bit;
                case "char": return SqlDbType.Char;
                case "date": return SqlDbType.Date;
                case "datetime": return SqlDbType.DateTime;
                case "datetime2": return SqlDbType.DateTime2;
                case "datetimeoffset": return SqlDbType.DateTimeOffset;
                case "numeric":
                case "decimal": return SqlDbType.Decimal;
                case "float": return SqlDbType.Float;
                case "image": return SqlDbType.Image;
                case "int": return SqlDbType.Int;
                case "money": return SqlDbType.Money;
                case "nchar": return SqlDbType.NChar;
                case "ntext": return SqlDbType.NText;
                case "nvarchar": return SqlDbType.NVarChar;
                case "real": return SqlDbType.Real;
                case "smalldatetime": return SqlDbType.SmallDateTime;
                case "smallint": return SqlDbType.SmallInt;
                case "smallmoney": return SqlDbType.SmallMoney;
                case "structured": return SqlDbType.Structured;
                case "text": return SqlDbType.Text;
                case "time": return SqlDbType.Time;
                case "timestamp": return SqlDbType.Timestamp;
                case "tinyint": return SqlDbType.TinyInt;
                case "udt": return SqlDbType.Udt;
                case "uniqueidentifier": return SqlDbType.UniqueIdentifier;
                case "varbinary": return SqlDbType.VarBinary;
                case "varchar": return SqlDbType.VarChar;
                case "sql_variant":
                case "variant": return SqlDbType.Variant;
                case "xml": return SqlDbType.Xml;
            }
            if (raise) throw new ArgumentException("SqlDbType not found: " + dbTypeName);
            return SqlDbType.Variant;
        }

        #region 添加
        private int _insert<T>(T model, ColumnTypes filter, ReturnTypes returnType)
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            List<string> list = EntityHelper.GetTableColumns(type, filter, null);
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append(tableName);
            builder.Append(" (");
            //if (returnType != ReturnTypes.Identity)
            //{
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}
            int count = list.Count;
            for (index = 0; index < count; index++)
            {

                builder.Append(list[index]);
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
            SqlParameter[] parms = new SqlParameter[count];
            for (index = 0; index < count; index++)
            {
                property = type.GetProperty(list[index]);
                parms[index] = new SqlParameter("@" + list[index], GetDbType(property.PropertyType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            if (((returnType == ReturnTypes.EffectRow) || (returnType == ReturnTypes.Identity)) || (returnType == ReturnTypes.None))
            {
                if (returnType == ReturnTypes.Identity)
                {
                    builder.Append("SELECT SCOPE_IDENTITY();");
                    object obj = SQLHelper.ExecuteScalar(connection, builder.ToString(), CommandType.Text, parms);
                    return obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
                }
                else
                {
                    int result = SQLHelper.NonQuery(connection, builder.ToString(), CommandType.Text, parms);
                    return result;
                }
            }
            return SQLHelper.NonQuery(connection, builder.ToString(), CommandType.Text, parms);
        }

        private int _insert<T>(T model, SqlConnection sqlconnection, SqlCommand command, SqlTransaction trans, ColumnTypes filter, ReturnTypes returnType)
        {
            int index = 0;
            Type type = typeof(T);
            string tableName = EntityHelper.GetTableName(type);
            List<string> list = EntityHelper.GetTableColumns(type, filter, null);
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append(tableName);
            builder.Append(" (");
            //if (returnType != ReturnTypes.Identity)
            //{
            //    builder.Append(list[0]);
            //    builder.Append(",");
            //}
            int count = list.Count;
            for (index = 0; index < count; index++)
            {

                builder.Append(list[index]);
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
            SqlParameter[] parms = new SqlParameter[count];
            for (index = 0; index < count; index++)
            {
                property = type.GetProperty(list[index]);
                parms[index] = new SqlParameter("@" + list[index], GetDbType(property.PropertyType, ref quote));
                parms[index].Value = property.GetValue(model, null);
            }
            if (((returnType == ReturnTypes.EffectRow) || (returnType == ReturnTypes.Identity)) || (returnType == ReturnTypes.None))
            {
                if (returnType == ReturnTypes.Identity)
                {
                    builder.Append("SELECT SCOPE_IDENTITY();");
                    object obj = SQLHelper.ExecuteScalar(builder.ToString(), sqlconnection, command, trans, CommandType.Text, parms);
                    return obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
                }
                else
                {
                    int result = SQLHelper.NonQuery(builder.ToString(), sqlconnection, command, trans, CommandType.Text, parms);
                    return result;
                }
            }
            return SQLHelper.NonQuery(builder.ToString(), sqlconnection, command, trans, CommandType.Text, parms);
        }
        #endregion
        private static void GetBaseSelect(StringBuilder sqlText, List<string> columns, string tableName, string where, int pagesize)
        {
            sqlText.Append("SELECT ");
            if (pagesize > 0)
            {
                sqlText.Append("top " + pagesize + " ");
            }
            if (columns != null && columns.Count > 0)
            {
                sqlText.Append(columns[0]);
                int count = columns.Count;
                for (int i = 1; i < count; i++)
                {
                    sqlText.Append(",");
                    sqlText.Append(columns[i]);
                }
            }
            else
            {
                sqlText.Append("*");
            }
            sqlText.Append(" FROM ");
            sqlText.Append(tableName);
            sqlText.Append(where);
        }
        #endregion
    }
}
