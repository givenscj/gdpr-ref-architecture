using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util
{
    public class ApplicationSettings
    {
        //private ArrayList settings = null;
        private Hashtable settings = null;
        //# Cache Required Stuff #

        public static string DatabaseConnection = "ConnectionString";

        /// <summary>Default Constructor for ApplicationSettings</summary>
        public ApplicationSettings()
        {
            //try to get out of cache first
            if (this.settings == null)
            {
                ICacheManager cm = CacheFactory.GetCacheManager();
                settings = (Hashtable)cm["_appSettings"];
            }

            //not there...load it up!
            if (this.settings == null)
            {
                this.settings = new Hashtable();
                this.LoadSettings();
            }
        }

        /// <summary>Default Constructor for ApplicationSettings</summary>
        /// <param name="Settings">Hashtable: a list of the Application Settings to store</param>
        public ApplicationSettings(Hashtable Settings)
        {
            if (null != Settings)
            {
                this.settings = Settings;

                this.PersistSettings();
            }
        }

        public bool ClearSettings()
        {
            bool _retVal = false;

            ICacheManager cm = CacheFactory.GetCacheManager();
            cm.Remove("_appSettings");

            return (_retVal);
        }

        /// <summary>Reads all the AppSetting nodes in the web.config</summary>
        /// <returns>bool: true/false if the operation was successful</returns>
        public bool LoadSettings()
        {
            IEnumerator _enumerator = ConfigurationSettings.AppSettings.GetEnumerator();

            while (_enumerator.MoveNext())
            {
                this.settings.Add(_enumerator.Current.ToString(), ConfigurationSettings.AppSettings[_enumerator.Current.ToString()]);
            }

            return (true);
        }

        /// <summary>The application settings value will be returned for the Key given</summary>
        /// <param name="Key">string: the Key to retrieve the value</param>
        /// <returns>string: the value of the given Key</returns>
        public string GetValue(string Key)
        {
            if (this.settings == null)
            {
                ICacheManager cm = CacheFactory.GetCacheManager();
                this.settings = (Hashtable)cm["appSettings"];
            }

            string _temp = "";

            if (null != settings[Key])
            {
                _temp = settings[Key].ToString();
            }

            return (_temp);
        }

        public bool PersistSettings()
        {
            bool _retVal = false;

            ICacheManager cm = CacheFactory.GetCacheManager();

            cm.Add("_appSettings", settings);

            return (_retVal);
        }
    }
}
