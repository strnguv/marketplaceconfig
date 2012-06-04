using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OEMStoreCombiner
{
    class Program
    {
        static void Main(string[] args)
        {
            string outdir = "/var/www";
            string[] locales = { "en-US", "es-US", "en-GB", "de-DE", "it-IT", "fr-FR", "es-ES", "en-CA",
                               "fr-CA", "en-AU", "fr-BE", "es-MX", "en-SG", "de-CH", "fr-CH", "it-CH",
                               "de-AT", "en-NZ", "en-IE", "en-HK", "zh-HK", "nl-BE", "ja-JP", "pt-BR", 
                               "ko-KR", "nl-NL", "pl-PL", "en-ZA", "sv-SE", "pt-PT", "da-DK", "nb-NO", 
                               "fi-FI", "cs-CZ", "es-CL", "hu-HU", "el-GR", "es-CO", "ru-RU", "en-IN", 
                               "zh-TW", "id-ID", "es-AR", "ms-MY", "en-PH", "es-PE" };
            string url = "http://catalog.zune.net/v3.2/{0}/stores/{1}/clientTypes/WinMobile%207.1/hubTypes/apps/hub?store={2}";
            string searchurl = "http://catalog.zune.net/v3.2/{0}/apps/?clientType=WinMobile%207.1&store={1}&orderby=downloadRank";
            string[] stores = { };//"HTC", "LGE", "Nokia", "SAMSUNG" };
            string[] stores_nohub = { "Acer", "Dell", "Futjitsu", "HTC", "LGE", "Nokia", "SAMSUNG", "ZTE" };
            
            // Check input
            if (args.Count() > 0)
                outdir = args[0];

            foreach (string locale in locales)
            {
                // Open the base XML
                Console.WriteLine("============================");
                Console.WriteLine(" Generating store for " + locale);
                Console.WriteLine("============================\n");
                XDocument xml = XDocument.Load("Skeleton.xml");
                xml.Declaration.Encoding = "utf-8";     // Running on Mono under linux defaults to utf-16, which the phone threw a fit over

                XElement editorialItems = xml.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}editorialItems").First();
                XElement applications = xml.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}applications").First();
                foreach (XElement updated in xml.Descendants("{http://www.w3.org/2005/Atom}updated"))
                    updated.SetValue(DateTime.UtcNow.ToString("o"));
                xml.Element("{http://www.w3.org/2005/Atom}feed").Element("{http://www.w3.org/2005/Atom}link").Attribute("href").SetValue("/oemstore_" + locale + ".xml");

                // Download OEM stores
                List<XElement> edItems = new List<XElement>();
                List<XElement> apps = new List<XElement>();

                foreach (string store in stores)
                {
                    Console.Write("Processing " + store + " hub... ");
                    XDocument storexml = XDocument.Load(string.Format(url, locale, store, store));

                    edItems.AddRange(storexml.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}editorialItem"));
                    apps.AddRange(storexml.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}application"));

                    Console.WriteLine(storexml.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}editorialItem").Count().ToString() + " found.");
                }

                // Search and parse the OEMs without hubs (ugh)
                foreach (string store in stores_nohub)
                {
                    Console.Write("Searching for apps by " + store + "... ");
                    XDocument searchxml = XDocument.Load(string.Format(searchurl, locale, store));

                    foreach (XElement entry in searchxml.Descendants("{http://www.w3.org/2005/Atom}entry"))
                    {
                        // Generate a new editorialItem for this app
                        edItems.Add(new XElement("{http://schemas.zune.net/catalog/apps/2008/02}editorialItem",
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}id", "urn:uuid:" + Guid.NewGuid().ToString()),  // Can't figure out how this ID plays in, sticking a random one in seems to work fine
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}link",
                                new XElement("{http://schemas.zune.net/catalog/apps/2008/02}type", "Application"),
                                new XElement("{http://schemas.zune.net/catalog/apps/2008/02}target", entry.Element("{http://www.w3.org/2005/Atom}id").Value.Substring(9))),
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}title", entry.Element("{http://www.w3.org/2005/Atom}title").Value),
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}text", entry.Element("{http://www.w3.org/2005/Atom}title").Value),    // Seaches don't return subtext, reuse the title again
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}sequenceNumber", 1),  // Doesn't matter, it will be changed
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}image",
                                new XElement("{http://schemas.zune.net/catalog/apps/2008/02}id", entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}image").Element("{http://schemas.zune.net/catalog/apps/2008/02}id").Value)),
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}backgroundImage",     // Search doesn't return background image, but it doesn't matter. Reuse icon
                                new XElement("{http://schemas.zune.net/catalog/apps/2008/02}id", entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}image").Element("{http://schemas.zune.net/catalog/apps/2008/02}id").Value))));

                        // Thankfully the search entry is pretty much identical to the application entry
                        apps.Add(new XElement("{http://schemas.zune.net/catalog/apps/2008/02}application",
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}id", entry.Element("{http://www.w3.org/2005/Atom}id").Value),
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}title", entry.Element("{http://www.w3.org/2005/Atom}title").Value),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}sortTitle"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}releaseDate"),
                            new XElement("{http://schemas.zune.net/catalog/apps/2008/02}shortDescription", entry.Element("{http://www.w3.org/2005/Atom}title").Value),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}version"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}averageUserRating"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}userRatingCount"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}averageLastInstanceUserRating"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}lastInstanceUserRatingCount"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}image"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}categories"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}tags"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}offers"),
                            entry.Element("{http://schemas.zune.net/catalog/apps/2008/02}publisher")
                            ));
                    }

                    Console.WriteLine(searchxml.Descendants("{http://www.w3.org/2005/Atom}entry").Count().ToString() + " found.");
                }

                // Sort based on name
                var sortedEd = from a in edItems orderby a.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}title").First().Value select a;
                var sortedApp = from a in apps orderby a.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}title").First().Value select a;

                Console.WriteLine("\nAdding applications:");

                int sequenceNumber = 2;
                foreach (XElement editorial in sortedEd)
                {
                    Console.Write(editorial.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}title").First().Value + "\t");
                    editorial.Descendants("{http://schemas.zune.net/catalog/apps/2008/02}sequenceNumber").First().SetValue(sequenceNumber.ToString());
                    sequenceNumber++;
                    editorialItems.Add(editorial);
                }

                foreach (XElement application in sortedApp)
                {
                    applications.Add(application);
                }

                Console.WriteLine("\nWriting oemstore_" + locale + ".xml to " +outdir + "\n");
                xml.Save(outdir + System.IO.Path.DirectorySeparatorChar + "oemstore_" + locale + ".xml");
            }
        }
    }
}
