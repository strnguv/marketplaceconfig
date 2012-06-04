using System;
using System.Globalization;

namespace MarketplaceConfig
{
    public class Region
    {
        public string DisplayName { get; private set; }
        public string EnglishName { get; private set; }
        public string NativeName { get; private set; }
        public string ShortName { get; private set; }

        public Uri Flag
        {
            get
            {
                return new Uri("/Resources/Flags/" + ShortName + ".png", UriKind.Relative);
            }
        }

        public Region(CultureInfo culture)
        {
            DisplayName = culture.DisplayName;
            EnglishName = culture.EnglishName;
            NativeName = culture.NativeName;
            ShortName = culture.ToString();
        }

        public Region(string cultureString)
        {
            // Special cases, where the WP7 SDK lacks certain cultures
            if (cultureString.Equals("en-HK"))
            {
                DisplayName = "English (Hong Kong S.A.R.)";
                EnglishName = "English (Hong Kong S.A.R.)";
                NativeName = "English (Hong Kong S.A.R.)";
                ShortName = "en-HK";
                return;
            }

            CultureInfo culture = new CultureInfo(cultureString);
            DisplayName = culture.DisplayName;
            EnglishName = culture.EnglishName;
            NativeName = culture.NativeName;
            ShortName = culture.ToString();
        }

        public Region()
        {

        }
    }
}
