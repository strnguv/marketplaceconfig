using System;
using System.Runtime.Serialization;
using System.Globalization;
using System.Windows.Media;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Net;
using System.Threading;
using System.Text;

namespace MarketplaceConfig
{
    [DataContract]
    public class Store
    {
        // Serializable members
        [DataMember] public int datVersion;
        [DataMember] public string displayName;
        [DataMember] public string storeName;
        [DataMember] public string storeID;
        [DataMember] public bool storeEnabled;
        [DataMember] public Uri storeImage;
        [DataMember] public string storeLocale;

        /// <summary>
        /// A user-friendly, printable string representation of the store
        /// </summary>
        public virtual string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        /// <summary>
        /// An image/icon to represent the store
        /// </summary>
        public virtual Uri DisplayImage
        {
            get
            {
                return storeImage;
            }
        }

        /// <summary>
        /// Returns a brush to color the text display with
        /// </summary>
        public virtual Brush DisplayColor
        {
            get
            {
                if (storeEnabled)
                    return new SolidColorBrush(Colors.White);
                else
                    return new SolidColorBrush(Colors.DarkGray);
            }
        }

        /// <summary>
        /// Returns the locale string in the format language-code/region (ex. en-US)
        /// </summary>
        public virtual string Locale
        {
            get
            {
                return storeLocale;
            }
        }

        public Store(string displayName, string storeName, string storeID,
            bool storeEnabled, Uri storeImage, string storeLocale)
        {
            this.displayName = displayName;
            this.storeName = storeName;
            this.storeID = storeID;
            this.storeEnabled = storeEnabled;
            this.storeImage = storeImage;
            this.storeLocale = storeLocale;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public XDocument generateXML()
        {
            return new XDocument(
                new XElement("ConfigurationFile",
                    new XAttribute("version", "1"),
                    new XElement("OEMStore",
                        new XElement("setting",
                            new XAttribute("id", "OEMName"),
                            storeID),
                        new XElement("setting",
                            new XAttribute("id", "OEMStoreName"),
                            storeName),
                        new XElement("setting",
                            new XAttribute("id", "OEMStoreID"),
                            storeID),
                        new XElement("setting",
                            new XAttribute("id", "OEMStoreEnabled"),
                            storeEnabled.ToString()))));
        }

        public static Store loadStore(string source)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Store));
            return (Store)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(source)));
        }

        public static Store loadStore(Uri source, bool local = false)
        {
            Stream stream = null;
            DataContractSerializer serializer = new DataContractSerializer(typeof(Store));

            if (local)
            {
                stream = System.Windows.Application.GetResourceStream(source).Stream;
            }
            else
            {
                
            }

            Store ret = (Store)serializer.ReadObject(stream);
            stream.Close();

            return ret;
        }

        public static string saveStore(Store store)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Store));            
            Stream stream = new MemoryStream();
            serializer.WriteObject(stream, store);
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType().Equals(typeof(string)))
                    return storeID.Equals((string)obj, StringComparison.InvariantCultureIgnoreCase);
                else if (obj.GetType().Equals(typeof(Store)))
                    return storeID.Equals(((Store)obj).storeID, StringComparison.InvariantCultureIgnoreCase);
            }

            return base.Equals(obj);
        }
    }
}
