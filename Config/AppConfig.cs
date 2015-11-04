namespace BS.Components.Data.Config
{
    using System;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Xml;

    public static class AppConfig
    {
        public static string GetConnection(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        public static int GetInt(string key)
        {
            string input = ConfigurationManager.AppSettings[key];
            if (!(((input != null) && (input.Length != 0)) && Regex.IsMatch(input, @"^\d+$")))
            {
                return 0;
            }
            return int.Parse(input);
        }

        public static XmlNode GetSection(string sectionName)
        {
            return (ConfigurationManager.GetSection(sectionName) as XmlNode);
        }

        public static string GetString(string key)
        {
            string str = ConfigurationManager.AppSettings[key];
            if (str == null)
            {
                return "";
            }
            return str;
        }
    }
}

