namespace BS.Components.Data.Config
{
    using System;
    using System.Configuration;
    using System.Xml;

    public class ConfigSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}

