using BS.Components.Data.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace BS.Components.Data.Config
{
    public class ConnConfig
    {
        public static string getConn
        {
            get
            {
                //string key = HttpContext.Current.Request.Url.Host;
                string key = System.Web.HttpContext.Current.Request.Url.Authority;
                string conn = getConnStr(key);
                return conn;
            }
        }
        /// <summary>
        /// 方便客户端程序共用
        /// </summary>
        /// <returns></returns>
        public static string getConnection()
        {
            try
            {
                string conn = "";
                if (System.Web.HttpContext.Current != null)
                {
                    string domain = System.Web.HttpContext.Current.Request.Url.Authority;
                    conn = getConnStr(domain);
                }
                else {
                    conn = getConnectionString("conn");
                }
                return conn;
            }
            catch (Exception exc)
            {
                LogHelper.writeLog(System.Environment.CurrentDirectory, "BS_Components_log_", exc.Message);
            }
            return "";
        }

        public static string getConnectionString(string key)
        {
            try
            {
                string connection = ConfigurationManager.ConnectionStrings["conn"] != null ? ConfigurationManager.ConnectionStrings["conn"].ToString() : "";
                if (key != null && key.Trim().Length > 0)
                {
                    connection = ConfigurationManager.ConnectionStrings[key] != null ? ConfigurationManager.ConnectionStrings[key].ToString() : "";
                }
                return connection;
            }
            catch { }
            return "";
        }
        #region 私有方法
        /// <summary>
        /// 根据AppSettings查，如果没有则取默认conn
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string getConnStr(string AppSettingsKey)
        {
            try
            {
                string connection = ConfigurationManager.ConnectionStrings["conn"] != null ? ConfigurationManager.ConnectionStrings["conn"].ToString() : "";
                if (AppSettingsKey != null && AppSettingsKey.Trim().Length > 0)
                {
                    string conn = ConfigurationManager.AppSettings[AppSettingsKey] != null ? ConfigurationManager.AppSettings[AppSettingsKey].ToString() : "";
                    if (conn.Length > 0)
                        connection = ConfigurationManager.ConnectionStrings[conn] != null ? ConfigurationManager.ConnectionStrings[conn].ToString() : "";
                }
                return connection;
            }
            catch { }
            return "";
        }
        #endregion
    }
}
