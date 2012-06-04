using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Globalization;

namespace MarketplaceConfig
{
    public class MOStore : Store
    {
        /// <summary>
        /// A user-friendly, printable string representation of the store's locale
        /// </summary>
        public string DisplaySubtext
        {
            get
            {
                if (string.IsNullOrEmpty(base.storeID))
                    return "Region-Free";
                return new CultureInfo(base.storeLocale).DisplayName;
            }
        }

        public MOStore(string displayName, string storeName, string storeID,
            bool storeEnabled, Uri storeImage, string storeLocale):
            base(displayName, storeName, storeID, storeEnabled, storeImage, storeLocale)
        {

        }

        public MOStore(Store x):
            base (x.displayName, x.storeName, x.storeID, x.storeEnabled, x.storeImage, x.storeLocale)
        {
            this.datVersion = x.datVersion;
        }

        /// <summary>
        /// Generates LKG_MOStoreConfig.xml
        /// </summary>
        /// <returns>LKG_MOStoreConfig.xml document</returns>
        public XDocument generateXML()
        {
            return new XDocument(
                new XElement("ConfigurationFile",
                    new XAttribute("version", "1"),
                    new XElement("MobileOperatorStore",
                        new XElement("setting",
                            new XAttribute("id", "MOStoreName"),
                            storeName),
                        new XElement("setting",
                            new XAttribute("id", "MOStoreID"),
                            storeID),
                        new XElement("setting",
                            new XAttribute("id", "MOStoreEnabled"),
                            storeEnabled.ToString()),
                        new XElement("ep",
                            new XAttribute("id", "MOStoreImage"),
                            new XAttribute("url", "http://image.catalog.{ZestDomain}/v3.2/{CatalogLocale}/stores/" + storeID + "/image?width=100")))));
        }

        /// <summary>
        /// Converts this MO Store to a generic Store object
        /// </summary>
        /// <returns>Store object</returns>
        public Store toStore()
        {
            Store ret = new Store(base.displayName, base.storeName, base.storeID,
                base.storeEnabled, base.storeImage, base.storeLocale);
            ret.datVersion = this.datVersion;
            return ret;
        }
    }
}
