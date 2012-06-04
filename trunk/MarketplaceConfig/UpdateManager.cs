using System;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Threading;

namespace MarketplaceConfig
{
    /// <summary>
    /// This class is used for downloading new or updated store configurations
    /// </summary>
    public class UpdateManager
    {
        private static UpdateManager instance = null;
        private WebClient downloader;

        private int downloading;
        private List<Store> oems;
        private List<Store> mos;
        private List<string> known;

        public event EventHandler UpdateComplete;
        protected void OnUpdateComplete(object sender, EventArgs e)
        {
            if (UpdateComplete != null)
                UpdateComplete(this, e);
        }

        private UpdateManager()
        {
            downloader = new WebClient();
            downloading = 0;
            oems = (List<Store>)SettingsManager.getInstance().loadValue("oems");
            mos = (List<Store>)SettingsManager.getInstance().loadValue("mos");
            known = new List<string>();
        }

        /// <summary>
        /// Get access to the singleton Update Manager class
        /// </summary>
        /// <returns>The instance of the class</returns>
        public static UpdateManager getInstance()
        {
            if (instance == null)
                instance = new UpdateManager();
            return instance;
        }

        /// <summary>
        /// Check if a store is available on the server
        /// </summary>
        /// <param name="storeID">StoreID to check</param>
        /// <returns>Boolean</returns>
        public bool knownStore(string storeID)
        {
            return known.Contains(storeID.ToLower());
        }

        /// <summary>
        /// Starts the update process in the background. Throws UpdateComplete event when finished.
        /// </summary>
        public void startUpdate()
        {
            downloader.DownloadStringCompleted += new DownloadStringCompletedEventHandler(downloader_DownloadStringCompleted);
            downloader.DownloadStringAsync(new Uri("http://winpho.foxingworth.com/marketplaceconfig/marketplaceconfig.xml"));
        }

        private void downloader_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Die quietly if the server could not be reached
            if (e.Error != null)
                return;

            XDocument xml = XDocument.Parse(e.Result);
            
            // Check OEMs
            foreach (XElement store in xml.Root.Element("oems").Descendants("store"))
            {
                if (downloadNeeded(store.Attribute("id").Value, int.Parse(store.Attribute("v").Value), ref oems))
                {
                    downloading++;
                    downloadStore(store.Value, store.Attribute("id").Value);
                }
                known.Add(store.Attribute("id").Value.ToLower());
            }

            // Check MO
            foreach (XElement store in xml.Root.Element("regions").Descendants("store"))
            {
                if (downloadNeeded(store.Attribute("id").Value, int.Parse(store.Attribute("v").Value), ref mos))
                {
                    downloading++;
                    downloadStore(store.Value, store.Attribute("id").Value, true);
                }
                known.Add(store.Attribute("id").Value.ToLower());
            }

            // All spawned web clients exist in this thread, start another thread to wait for them otherwise they'll
            // lock up on the sleep statements
            new Thread(new ThreadStart(waitThread)).Start();
        }

        private void waitThread()
        {
            // Wait until everything is done downloading before throwing the update complete event
            while (downloading > 0)
                Thread.Sleep(100);

            SettingsManager.getInstance().saveValue("oems", oems);
            SettingsManager.getInstance().saveValue("mos", mos);

            // Ensure the event is thrown on the UI event
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                OnUpdateComplete(this, EventArgs.Empty);
            });
        }

        private void downloadStore(string source, string storeID, bool MO = false)
        {
            WebClient storeClient = new WebClient();

            storeClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((object s, DownloadStringCompletedEventArgs e) => {
                if (e.Error != null)
                {
                    downloading--;
                    return;
                }
                
                Store store = Store.loadStore(e.Result);

                // TODO: Fix this ugly mess. I wish passing by reference worked in anonymous methods
                if (MO)
                {
                    lock (mos)
                    {
                        var test = from st in mos 
                                   where st.storeID.Equals(store.storeID, StringComparison.InvariantCultureIgnoreCase)
                                   select st;

                        if (test.Count() > 0)
                            mos.Remove(test.First());

                        mos.Add(store);
                    }
                }
                else
                {
                    lock (oems)
                    {
                        var test = from st in oems where st.storeID.Equals(store.storeID, StringComparison.InvariantCultureIgnoreCase) select st;

                        if (test.Count() > 0)
                            oems.Remove(test.First());

                        oems.Add(store);
                    }
                }
                downloading--;
            });
            storeClient.DownloadStringAsync(new Uri("http://winpho.foxingworth.com/marketplaceconfig/" + source));
        }

        //
        private bool downloadNeeded(string storeID, int version, ref List<Store> collection)
        {
            var result = from s in collection where s.storeID.Equals(storeID, StringComparison.InvariantCultureIgnoreCase) select s;

            if (result.Count() == 0)
                return true;
            else
                return (result.First().datVersion < version);
        }
    }
}
