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
                string conn = getConnectionString(key);
                return conn;
            }
        }


        public static string getConnectionString(string key)
        {
            try
            {
                string connection = ConfigurationManager.ConnectionStrings["conn"] != null ? ConfigurationManager.ConnectionStrings["conn"].ToString() : "";
                if (key != null && key.Trim().Length > 0)
                {
                    string conn = ConfigurationManager.AppSettings[key] != null ? ConfigurationManager.AppSettings[key].ToString() : "";
                    if (conn.Length > 0)
                        connection = ConfigurationManager.ConnectionStrings[conn] != null ? ConfigurationManager.ConnectionStrings[conn].ToString() : "";
                }
                return connection;
            }
            catch { }
            return "";
        }
    }
}
