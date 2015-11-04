namespace BS.Components.Data.DataProvider
{
    using System;
    using System.IO;
    using System.Xml;

    public class XmlHelper
    {
        private XmlDocument _xmlDoc;

        public XmlHelper(string fileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(path))
            {
                throw new Exception("Xml file \"" + fileName + "\" does not exist.");
            }
            this._xmlDoc = new XmlDocument();
            this._xmlDoc.Load(path);
        }

        public XmlNodeList SelectNodes(string xPath)
        {
            return this._xmlDoc.SelectNodes(xPath);
        }

        public XmlNode SelectSingleNode(string xPath)
        {
            return this._xmlDoc.SelectSingleNode(xPath);
        }
    }
}

