using System;
using System.Linq;
using System.Xml.Linq;
using Homebrew;
using Homebrew.IO;
using System.Globalization;
using System.IO.IsolatedStorage;

namespace MarketplaceConfig
{
    /// <summary>
    /// This static class is responsible for reading and manipulating the market settings.
    /// </summary>
    public class MarketWorker
    {

        /// <summary>
        /// Creates a backup of the current market configuration or updates it if is missing anything
        /// </summary>
        /// <returns>True if created or updated, false if the backup already existed</returns>
        public static bool createBackup()
        {
            bool changed = false;

            // Create the backup folder if it doesn't exist
            if (!WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\Backup"))
                WP7RootToolsSDK.FileSystem.CreateFolder(@"\My Documents\Zune\PimentoCache\Keepers\Backup");

            // LKG_MOConfig contains settings related to the functioning of the mobile operator (payment, max download, etc)
            if (!WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_MOConfig.xml") &&
                WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOConfig.xml"))
            {
                WP7RootToolsSDK.FileSystem.CopyFile(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOConfig.xml",
                    @"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_MOConfig.xml");
                changed = true;
            }

            // LKG_MOStoreConfig controls which mobile operator store the marketplace shows
            if (!WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_MOStoreConfig.xml") &&
                WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml"))
            {
                WP7RootToolsSDK.FileSystem.CopyFile(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml",
                    @"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_MOStoreConfig.xml");
                changed = true;
            }

            // LKG_OEMStoreConfig controls which OEM store the marketplace shows
            if (!WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_OEMStoreConfig.xml") &&
                WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml"))
            {
                WP7RootToolsSDK.FileSystem.CopyFile(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml",
                    @"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_OEMStoreConfig.xml");
                changed = true;
            }

            // LKG_TunerConfig controls many aspects of the marketplace, mostly where it fetches information from
            // (this will be a powerful thing to edit once I figure out more about it)
            if (!WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_TunerConfig.xml") &&
                WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml"))
            {
                WP7RootToolsSDK.FileSystem.CopyFile(@"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml",
                    @"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_TunerConfig.xml");
                changed = true;
            }

            // Backup registry entries
            if (!SettingsManager.getInstance().valueExists("xuid"))
            {
                SettingsManager.getInstance().saveValue("xuid", getMarketRegion().ToString());
                changed = true;
            }

            // Return if anything was backed up or not
            return changed;
        }

        /// <summary>
        /// Reads the current market region from the registry
        /// </summary>
        /// <returns>Currently set market locale (in region shortcode, ie. EN-US)</returns>
        public static string getMarketRegion()
        {
            string xuid = WP7RootToolsSDK.Registry.GetStringValue(WP7RootToolsSDK.RegistryHyve.LocalMachine, 
                @"SOFTWARE\Microsoft\Zune\Settings", "XuidLocale");
            return xuid;
        }

        /// <summary>
        /// Sets the market region in the registry
        /// </summary>
        /// <param name="region">Region to set</param>
        public static void setMarketRegion(Region region)
        {
            string xuid = region.ShortName.ToUpper();
            WP7RootToolsSDK.Registry.SetStringValue(WP7RootToolsSDK.RegistryHyve.LocalMachine, 
                @"SOFTWARE\Microsoft\Zune\Settings", "XuidLocale", xuid);
        }

        /// <summary>
        /// Reads the maximum file size to download over a 3G/4G network connection
        /// </summary>
        /// <returns>Maxmium file size as double</returns>
        public static double getMaxDownload()
        {
            // Make sure there is a change
            double max = double.Parse(readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOConfig.xml", "{http://schemas.microsoft.com/wps/mobi/2011}setting", "id", "MaxDownloadSize"));
            return max;
        }

        /// <summary>
        /// Sets the maximum file size to download over a 3G/4G network connection
        /// </summary>
        /// <param name="max">Maxmium file size as double</param>
        public static void setMaxDownload(double max)
        {
            setXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOConfig.xml",
                "{http://schemas.microsoft.com/wps/mobi/2011}setting", "id", "MaxDownloadSize", max.ToString("0"));
        }

        /// <summary>
        /// Generates a Store object from the current marketplace settings
        /// </summary>
        /// <returns>Store object reflecting the marketplace settings</returns>
        public static Store generateOEMStore()
        {
            string displayName = readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", "setting", "id", "OEMName");
            string storeName = readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", "setting", "id", "OEMStoreName");
            string storeID = readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", "setting", "id", "OEMStoreID");
            bool storeEnabled = readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", "setting", "id", "OEMStoreEnabled").Equals("true");
            string locale = getMarketRegion();

            return generateStore(displayName, storeName, storeID, storeEnabled, 
                new Uri("http://image.catalog.zune.net/v3.2/" + locale + "/stores/" + storeID + "/image?width=100", UriKind.Absolute), 
                locale);
        }

        /// <summary>
        /// Generates a MO Store object from the current marketplace settings
        /// </summary>
        /// <returns>MOStore object reflecting the marketplace settings</returns>
        public static MOStore generateMOStore()
        {
            string displayName = readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", "setting", "id", "MOStoreName");
            string storeID = readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", "setting", "id", "MOStoreID");
            bool storeEnabled = readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", "setting", "id", "MOStoreEnabled").Equals("true");
            string locale = getMarketRegion();

            return new MOStore(generateStore(displayName, displayName, storeID, storeEnabled,
                new Uri("http://image.catalog.zune.net/v3.2/" + locale + "/stores/" + storeID + "/image?width=100", UriKind.Absolute),
                locale));
        }

        private static Store generateStore(string displayName, string storeName, string storeID, bool storeEnabled,
            Uri storeImage, string storeLocale)
        {
            return new Store(storeName, storeName, storeID, storeEnabled, storeImage, storeLocale);
        }

        /// <summary>
        /// Returns the store ID of the current OEM marketplace
        /// </summary>
        /// <returns>Store ID as a string</returns>
        public static string getOEMStore()
        {
            try
            {
                return readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", "setting", "id", "OEMStoreID");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Sets the provided marketplace as the current
        /// </summary>
        /// <param name="store">OEM Marketplace to set</param>
        public static void setOEMStore(Store store)
        {
            // Unlock the file if marked as read-only
            if (WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml"))
            {
                FileAttributes attributes = File.GetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml");
                if (attributes.ReadOnly)
                    File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive));
            }

            // Overwrite the file with the desired market
            File.WriteAllBytes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.new",
                System.Text.Encoding.UTF8.GetBytes(store.generateXML().ToString()));
            File.Copy(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.new", @"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml");
            File.Delete(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.new");

            // Set the correct attributes
            if ((bool)SettingsManager.getInstance().loadValue("permanent"))
                File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive | FileAttributesEnum.Readonly));
            else
                File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive));
        
            // Check if this is the all OEMs market
            if (string.IsNullOrEmpty(store.storeID))
            {
                /* =====================================================
                 * NOTE TO THOSE ADDING THIS FUNCTIONALITY TO THEIR APPS
                 * =====================================================
                 * Feel free to review what I do and incorporate it into your own apps. However, you'll notice
                 * that I am using my own server (foxingworth.com) for hosting the new store files. Please host
                 * these files on YOUR OWN server and don't simply have your app point towards mine. I included the
                 * source of the program I wrote to generate these modified files, so use that program or use the source
                 * to write your own. Until the day that I get my dedicated server for free, I would rather be in control
                 * of the traffic to it. Thank you.
                 */

                // Have the marketplace look at my server for the OEM hub
                setXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml", "ep", "id", "OEMApplications",
                    "http://winpho.foxingworth.com/oemstore/oemstore_{CatalogLocale}.xml", true, "url");

                // Tweak the other application URLs to allow all oem stores
                setXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml", "ep", "id", "Application",
                    "http://catalog.{ZestDomain}/v3.2/{CatalogLocale}/apps/{objectid}?clientType={YPV}&store=Zest&store={MOStoreID}&store=Nokia&store=HTC&store=LGE&store=SAMSUNG&store=Dell&store=Fujitsu&store=Acer&;store=ZTE{urltail}",
                    true, "url");
                /*  Can't just tack on extra stores like in 'Application', it breaks search for some reason...
                setXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml", "ep", "id", "ApplicationSearch",
                    "http://catalog.{ZestDomain}/v3.2/{CatalogLocale}/apps?q={query}&chunkSize={appsearchchunksize}&clientType={YPV}&store=Zest&store={MOStoreID}&store=Nokia&store=HTC&store=LGE&store=SAMSUNG&store=Dell&store=Fujitsu&store=Acer&store=ZTE{urltail}",
                    true, "url");*/
            }
        }

        /// <summary>
        /// Returns the store ID of the current MO marketplace
        /// </summary>
        /// <returns>Store ID as a string</returns>
        public static string getMOStore()
        {
            try
            {
                return readXMLValue(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", "setting", "id", "MOStoreID");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Sets the provided marketplace as the current
        /// </summary>
        /// <param name="store">MO Marketplace to set</param>
        public static void setMOStore(MOStore store)
        {
            // Unlock the file if marked as read-only
            if (WP7RootToolsSDK.FileSystem.FileExists(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml"))
            {
                FileAttributes attributes = File.GetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml");
                if (attributes.ReadOnly)
                    File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive));
            }

            // Overwrite the file with the desired market
            File.WriteAllBytes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.new",
                System.Text.Encoding.UTF8.GetBytes(store.generateXML().ToString()));
            File.Copy(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.new", @"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml");
            File.Delete(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.new");

            // Set the correct attributes
            if ((bool)SettingsManager.getInstance().loadValue("permanent"))
                File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive | FileAttributesEnum.Readonly));
            else
                File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive));
        }

        /// <summary>
        /// Reads the specified value from an XML document
        /// </summary>
        /// <param name="fileSource">Path to the XML document</param>
        /// <param name="elementName">The name of the element to read the value of</param>
        /// <param name="attributeName">The element's attribute name (for selecting proper element)</param>
        /// <param name="attributeValue">The value of the attribute to search for</param>
        /// <returns>The value of the specified element</returns>
        private static string readXMLValue(string fileSource, string elementName, string attributeName, string attributeValue)
        {
            if (!WP7RootToolsSDK.FileSystem.FileExists(fileSource))
                throw new Exception("XML Error: " + fileSource + " does not exist");

            // Read and parse the XML
            FileStream source = File.Open(fileSource, FileAccess.Read, FileShare.Read, CreationDisposition.OpenExisting);
            XDocument xml = XDocument.Load(source);
            source.Close();

            // Find the asked for value
            var value = from q in xml.Descendants(elementName)
                        //where q.Attribute("id").Value.Equals("MaxDownloadSize")
                        where q.Attribute(attributeName).Value.Equals(attributeValue)
                        select q.Value;

            // Make sure one (and only one) result was found
            if (value.Count() == 0)
                throw new Exception("XML Error: " + elementName + " not found in " + fileSource);
            else if (value.Count() > 1)
                throw new Exception("XML Error: " + elementName + " found more than once in " + fileSource);

            return value.First();
        }

        /// <summary>
        /// Sets the value of a specificed element or attribute in an XML document
        /// </summary>
        /// <param name="fileSource">The XML document to edit</param>
        /// <param name="elementName">The element name to edit</param>
        /// <param name="searchAttributeName">The attribute name of the element</param>
        /// <param name="searchAttributeValue">The attribute value to search for</param>
        /// <param name="newValue">The new value to set</param>
        /// <param name="setAttribute">True if this is an attribute value, false for element value</param>
        /// <param name="attributeName">The name of the attribute to set the value of</param>
        private static void setXMLValue(string fileSource, string elementName, string searchAttributeName, 
            string searchAttributeValue, string newValue, bool setAttribute = false, string attributeName = "")
        {
            if (!WP7RootToolsSDK.FileSystem.FileExists(fileSource))
                throw new Exception("XML Error: " + fileSource + " does not exist");

            // Read and parse the XML
            FileStream source = File.Open(fileSource, FileAccess.Read, FileShare.Read, CreationDisposition.OpenExisting);
            XDocument xml = XDocument.Load(source, LoadOptions.PreserveWhitespace);
            source.Close();

            // Find the asked for value
            var value = from q in xml.Descendants(elementName)
                        where q.Attribute(searchAttributeName).Value.Equals(searchAttributeValue)
                        select q;

            // Make sure one (and only one) result was found
            if (value.Count() == 0)
                throw new Exception("XML Error: " + elementName + " not found in " + fileSource);
            else if (value.Count() > 1)
                throw new Exception("XML Error: " + elementName + " found more than once in " + fileSource);

            // Write out the change
            if (setAttribute)
                value.First().SetAttributeValue(attributeName, newValue);
            else
                value.First().SetValue(newValue);

            File.WriteAllBytes(fileSource.Replace(".xml", ".new"), System.Text.Encoding.UTF8.GetBytes(xml.ToString()));
            File.Copy(fileSource.Replace(".xml", ".new"), fileSource);
            File.Delete(fileSource.Replace(".xml", ".new"));

            // Set the correct attributes
            if ((bool)SettingsManager.getInstance().loadValue("permanent"))
                File.SetAttributes(fileSource, new FileAttributes(FileAttributesEnum.Archive | FileAttributesEnum.Readonly));
            else
                File.SetAttributes(fileSource, new FileAttributes(FileAttributesEnum.Archive));
        }
    }
}
