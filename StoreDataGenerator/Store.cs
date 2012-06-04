using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace MarketplaceConfig
{
    [DataContract]
    public class Store
    {
        // Serializable members
        [DataMember] protected int datVersion;
        [DataMember] protected string displayName;
        [DataMember] protected string storeName;
        [DataMember] protected string storeID;
        [DataMember] protected bool storeEnabled;
        [DataMember] protected Uri storeImage;
        [DataMember] protected string storeLocale;

        public Store(int version, string displayName, string storeName, string storeID,
            bool storeEnabled, Uri storeImage, CultureInfo storeLocale)
        {
            this.datVersion = version;
            this.displayName = displayName;
            this.storeName = storeName;
            this.storeID = storeID;
            this.storeEnabled = storeEnabled;
            this.storeImage = storeImage;
            this.storeLocale = storeLocale.ToString();
        }
    }
}
