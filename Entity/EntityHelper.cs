namespace BS.Components.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    public class EntityHelper
    {

        /// <summary>
        /// 后加，方便外围调用
        /// </summary>
        /// <param name="customColumns"></param>
        /// <param name="sdr"></param>
        /// <returns></returns>
        public static T GetDataReaderObject<T>(string customColumns, IDataReader sdr) where T : class
        {
            Type type = typeof(T);
            string typeName = type.Namespace + "." + type.Name;
            List<string> columns = EntityHelper.GetTableColumns(type, ColumnTypes.Extend, customColumns);
            return GetDataReaderObject<T>(type, typeName, columns, sdr);
        }

        public static T GetDataReaderObject<T>(Type type, string typeName, List<string> columns, IDataReader sdr) where T : class
        {

            T local = type.Assembly.CreateInstance(typeName) as T;

            //for (int j = 0; j < sdr.FieldCount; j++)
            //{
            //    string dr_FieldName = sdr.GetName(j);
            //    bool b = false;
            //    foreach (string str in columns)
            //    {
            //        if (!sdr.IsDBNull(j) && dr_FieldName.Trim().ToLower().Equals(str.Trim().ToLower()))
            //        {
            //            b = true;
            //            break;
            //        }
            //    }
            //    if (b)
            //        type.GetProperty(dr_FieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(local, sdr.GetValue(j), null);
            //}

            //int i = 0;
            foreach (string str in columns)
            {
                //if (!sdr.IsDBNull(i))
                //{
                //    //object columns_val = sdr.GetValue(i);
                //    object columns_val = sdr[str];
                //    type.GetProperty(str, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(local, columns_val, null);
                //}
                //i++;

                object columns_val = sdr[str];
                if (columns_val != DBNull.Value)
                    type.GetProperty(str, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(local, columns_val, null);
            }
            return local;
        }

        //public static T GetDataReaderObject<T>(Type type, string typeName, List<string> columns, IDataReader sdr) where T : class
        //{
        //    int i = 0;
        //    T local = type.Assembly.CreateInstance(typeName) as T;
        //    foreach (string str in columns)
        //    {
        //        if (!sdr.IsDBNull(i))
        //        {
        //            PropertyInfo propertyInfo = type.GetProperty(str, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        //            string info_FieldName = propertyInfo.Name;//当前实体字段名
        //            string dr_FieldName = sdr.GetName(i);
        //            if (sdr.GetSchemaTable().Select("ColumnName='" + str + "'").Length > 0 && dr_FieldName.Equals(info_FieldName))
        //            {
        //                //type.GetProperty(str, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(local, sdr.GetValue(i), null);
        //                propertyInfo.SetValue(local, sdr.GetValue(i), null);
        //            }
        //        }
        //        i++;
        //    }
        //    return local;
        //}

        public static PropertyAttribute GetProperty(Type type)
        {
            return (PropertyAttribute)type.GetCustomAttributes(false)[0];
        }

        public static List<string> GetTableColumns(Type type, ColumnTypes filter, string customColumns)
        {
            //string key = type.FullName + "_" + filter.ToString() + "_" + customColumns;
            List<string> columns = null;// ObjectCache.GetColumns(key);
            if (columns == null)
            {
                columns = new List<string>();
                if (customColumns == "*")
                {
                    //columns.Add("*");
                    forcolumns(type, filter, columns);
                }
                else if ((customColumns != null) && (customColumns.Length > 0))
                {
                    string[] strArray = customColumns.Split(new char[] { ',' });
                    foreach (string str2 in strArray)
                    {
                        columns.Add(str2.Trim());
                    }
                }
                else
                {
                    forcolumns(type, filter, columns);
                }
                //ObjectCache.SetColumns(key, columns);
            }
            return columns;
        }

        private static void forcolumns(Type type, ColumnTypes filter, List<string> columns)
        {
            PropertyInfo[] properties = type.GetProperties();
            if (properties != null)
            {
                bool flag = false;
                foreach (PropertyInfo info in properties)
                {
                    if (Attribute.IsDefined(info, typeof(PropertyAttribute)))
                    {
                        PropertyAttribute customAttribute = (PropertyAttribute)Attribute.GetCustomAttribute(info, typeof(PropertyAttribute));
                        string[] strArray2 = customAttribute.ColumnType.ToString().Split(new char[] { ',' });
                        foreach (string str3 in strArray2)
                        {
                            ColumnTypes types = (ColumnTypes)Enum.Parse(typeof(ColumnTypes), str3);
                            if (types == (filter & types))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            flag = false;
                            continue;
                        }
                    }
                    columns.Add(info.Name);
                }
            }
        }

        public static PropertyInfo GetTableIdentity(Type type)
        {
            string fullName = type.FullName;
            PropertyInfo propertyInfo = ObjectCache.GetPropertyInfo(fullName);
            if (propertyInfo == null)
            {
                PropertyAttribute customAttribute = null;
                foreach (PropertyInfo info2 in type.GetProperties())
                {
                    if (Attribute.IsDefined(info2, typeof(PropertyAttribute)))
                    {
                        customAttribute = (PropertyAttribute)Attribute.GetCustomAttribute(info2, typeof(PropertyAttribute));
                        if (ColumnTypes.Identity == (ColumnTypes.Identity & customAttribute.ColumnType))
                        {
                            propertyInfo = info2;
                        }
                    }
                }
                ObjectCache.SetPropertyInfo(fullName, propertyInfo);
            }
            return propertyInfo;
        }

        public static string GetTableName(Type type)
        {
            string key = "TN_" + type.FullName;
            string tableName = ObjectCache.GetString(key);
            if (tableName == null)
            {
                object[] objarr = type.GetCustomAttributes(false);
                tableName = ((PropertyAttribute)objarr[0]).TableName;
                ObjectCache.SetString(key, tableName);
            }
            return tableName;
        }
    }
}

