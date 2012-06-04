using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Linq;

namespace MarketplaceConfig
{
    /// <summary>
    /// This class provides a thread-safe way to read and write settings.
    /// </summary>
    public class SettingsManager
    {
        private static SettingsManager instance = null;

        private Dictionary<string, object> defaults = new Dictionary<string, object>();

        /// <summary>
        /// Get access to the singleton Settings Manager class
        /// </summary>
        /// <returns>Return the instance of the class</returns>
        public static SettingsManager getInstance()
        {
            if (instance == null)
                instance = new SettingsManager();
            return instance;
        }

        private SettingsManager()
        {
            // Generate default values
            defaults.Add("autoupdate", true);
            defaults.Add("permanent", false);
            defaults.Add("autoregion", true);
            defaults.Add("hideforeign", true);
            defaults.Add("submitasked", false);

            List<Store> emptyOEMs = new List<Store>();
            emptyOEMs.Add(Store.loadStore(new Uri("Resources/Stores/alloems.dat", UriKind.Relative), true));
            defaults.Add("emptyoems", emptyOEMs);

            List<MOStore> emptyMOs = new List<MOStore>();
            emptyMOs.Add(new MOStore(Store.loadStore(new Uri("Resources/Stores/unlocked.dat", UriKind.Relative), true)));
            defaults.Add("emptymos", emptyMOs);

            // OEM list
            if (!valueExists("oems"))
            {
                List<Store> oems = new List<Store>();
                oems.Add(Store.loadStore(new Uri("Resources/Stores/alloems.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/acer.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/dell.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/fujitsu.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/htc.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/lge.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/nokia.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/samsung.dat", UriKind.Relative), true));
                oems.Add(Store.loadStore(new Uri("Resources/Stores/zte.dat", UriKind.Relative), true));
                defaults.Add("oems", oems);
            }
            
            // MO list
            if (!valueExists("mos"))
            {
                List<Store> mos = new List<Store>();
                mos.Add(Store.loadStore(new Uri("Resources/Stores/unlocked.dat", UriKind.Relative), true));
                mos.Add(MarketWorker.generateMOStore().toStore());
                defaults.Add("mos", mos);
            }

            // Region list
            List<string> regions = new List<string>();
            // Original markets
            regions.Add("en-US");   regions.Add("es-US");   regions.Add("en-GB");   regions.Add("de-DE");
            regions.Add("it-IT");   regions.Add("fr-FR");   regions.Add("es-ES");   regions.Add("en-CA");
            regions.Add("fr-CA");   regions.Add("en-AU");   regions.Add("fr-BE");   regions.Add("es-MX");
            regions.Add("en-SG");   regions.Add("de-CH");   regions.Add("fr-CH");   regions.Add("it-CH");
            regions.Add("de-AT");   regions.Add("en-NZ");   regions.Add("en-IE");   regions.Add("en-HK");
            regions.Add("zh-HK");
            // New mango markets
            /*regions.Add("nl-BE"); regions.Add("ja-JP"); regions.Add("pt-BR"); regions.Add("ko-KR");
            regions.Add("nl-NL"); regions.Add("pl-PL"); regions.Add("en-ZA"); regions.Add("sv-SE");
            regions.Add("pt-PT"); regions.Add("da-DK"); regions.Add("nb-NO"); regions.Add("fi-FI");
            regions.Add("cs-CZ"); regions.Add("es-CL"); regions.Add("hu-HU"); regions.Add("el-GR");
            regions.Add("es-CO"); regions.Add("ru-RU"); regions.Add("en-IN"); regions.Add("zh-TW");
            regions.Add("id-ID"); regions.Add("es-AR"); regions.Add("ms-MY"); regions.Add("en-PH");
            regions.Add("es-PE");
            // New tango markets
            regions.Add("zh-CN"); regions.Add("bg-BG"); regions.Add("es-CR"); regions.Add("hr-HR");
            regions.Add("et-EE"); regions.Add("is-IS"); regions.Add("lv-LV"); regions.Add("lt-LT");
            regions.Add("ro-RO"); regions.Add("sk-SK"); regions.Add("sl-SI"); regions.Add("tr-TR");
            regions.Add("uk-UA"); regions.Add("es-VE");
            // Second round of new tango markets
            /*regions.Add("ar-BH"); regions.Add("ar-IQ"); regions.Add("he-IL"); regions.Add("kk-KZ");
            regions.Add("ar-QA"); regions.Add("ar-SA"); regions.Add("th-TH"); regions.Add("ar-AE");
            regions.Add("vi-VN");*/     // Not supported by CultureInfo on WP7? (weird)*/
            
            defaults.Add("regions", regions);

        }

        /// <summary>
        /// Loads a saved setting.
        /// </summary>
        /// <param name="name">The setting name to load</param>
        /// <returns>The desired setting, or the default value if this setting is not saved</returns>
        public object loadValue(string name)
        {
            lock (IsolatedStorageSettings.ApplicationSettings)
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains(name))
                {
                    if (!defaults.ContainsKey(name))
                        throw new Exception(string.Format("SettingsManager: No default value available for {0}", name));

                    return defaults[name];
                }

                return IsolatedStorageSettings.ApplicationSettings[name];
            }
        }

        /// <summary>
        /// Saves a value to the settings. Make sure the object is serializable.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="value">The object to store</param>
        public void saveValue(string name, object value)
        {
            lock (IsolatedStorageSettings.ApplicationSettings)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(name))
                    IsolatedStorageSettings.ApplicationSettings.Remove(name);

                IsolatedStorageSettings.ApplicationSettings.Add(name, value);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// Check if setting has been saved before.
        /// </summary>
        /// <param name="name">The name of the setting to check.</param>
        /// <returns>True if the settings exists, false if the return value with be the default</returns>
        public bool valueExists(string name)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(name);
        }

        /// <summary>
        /// Erases all saved settings and destroys the manager instance. Use for debugging.
        /// </summary>
        public void destroySettings()
        {
            IsolatedStorageSettings.ApplicationSettings.Clear();
            IsolatedStorageSettings.ApplicationSettings.Save();
            instance = null;
        }
    }
}
