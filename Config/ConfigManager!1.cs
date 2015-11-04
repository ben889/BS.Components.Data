namespace BS.Components.Data.Config
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using BS.Components.Data.DataProvider;

    public class ConfigManager<T> where T: class
    {
        private IConfigInfo _configInfo;
        private string _fullfile;
        private DateTime _lastDateTime;
        private Timer _timer;
        private Dictionary<string, object> _values;
        private static object loadConfig_lock;

        static ConfigManager()
        {
            ConfigManager<T>.loadConfig_lock = new object();
        }

        public ConfigManager(string configName, string filename)
        {
            this._values = new Dictionary<string, object>();
            string baseDirectory = "";
            if ((configName != null) && (configName.Length > 0))
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings[configName];
            }
            else
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            this._fullfile = Path.Combine(baseDirectory, filename);
            if (!File.Exists(this._fullfile))
            {
                throw new FileNotFoundException(string.Format("{0} is missing!", filename), this._fullfile);
            }
            this.LoadData(this._fullfile, false);
            this._timer = new Timer(new TimerCallback(this.TimerClickCallback), null, -1, -1);
            this._timer.Change(0x1388, 0x1388);
        }

        public object GetProperty(string name)
        {
            if (!this._values.ContainsKey(name))
            {
                this._values.Add(name, typeof(T).GetProperty(name).GetValue(this._configInfo, null));
            }
            return this._values[name];
        }

        private IConfigInfo Load(Type type, string fullfile)
        {
            lock (ConfigManager<T>.loadConfig_lock)
            {
                return (IConfigInfo) SerializationHelper.Load(type, fullfile);
            }
        }

        public virtual void LoadData(string configfile, bool forceToLoad)
        {
            if ((this._configInfo == null) || forceToLoad)
            {
                this._configInfo = this.Load(typeof(T), configfile);
                this._values.Clear();
                this._lastDateTime = File.GetLastWriteTime(configfile);
            }
        }

        public bool Save(IConfigInfo configinfo)
        {
            return SerializationHelper.Save(configinfo, this._fullfile);
        }

        private void TimerClickCallback(object state)
        {
            this._timer.Change(-1, -1);
            if (File.GetLastWriteTime(this._fullfile) != this._lastDateTime)
            {
                this.LoadData(this._fullfile, true);
            }
            this._timer.Change(0x1388, 0x1388);
        }

        public IConfigInfo ConfigInfo
        {
            get
            {
                return this._configInfo;
            }
        }
    }
}

