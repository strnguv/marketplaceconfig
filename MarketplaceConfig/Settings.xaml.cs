using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Homebrew.IO;
using WP7RootToolsSDK;

namespace MarketplaceConfig
{
    public partial class Page1 : PhoneApplicationPage
    {

        public Page1()
        {
            InitializeComponent();

            toggleAutoUpdate.IsChecked = (bool)SettingsManager.getInstance().loadValue("autoupdate");
            togglePermanent.IsChecked = (bool)SettingsManager.getInstance().loadValue("permanent");
            toggleAutoRegion.IsChecked = (bool)SettingsManager.getInstance().loadValue("autoregion");
            toggleHideForeign.IsChecked = (bool)SettingsManager.getInstance().loadValue("hideforeign");
        }

        private void toggleAutoUpdate_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.getInstance().saveValue("autoupdate", toggleAutoUpdate.IsChecked);
        }

        private void togglePermanent_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.getInstance().saveValue("permanent", togglePermanent.IsChecked);
        }

        private void toggleHideForeign_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.getInstance().saveValue("hideforeign", toggleHideForeign.IsChecked);
        }

        private void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            // Unlock files if set as read-only
            if (Homebrew.IO.File.GetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOConfig.xml").ReadOnly)
                Homebrew.IO.File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOConfig.xml", new FileAttributes(FileAttributesEnum.Archive));
            if (Homebrew.IO.File.GetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml").ReadOnly)
                Homebrew.IO.File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive));
            if (Homebrew.IO.File.GetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml").ReadOnly)
                Homebrew.IO.File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml", new FileAttributes(FileAttributesEnum.Archive));
            if (Homebrew.IO.File.GetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml").ReadOnly)
                Homebrew.IO.File.SetAttributes(@"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml", new FileAttributes(FileAttributesEnum.Archive));

            // Copy originals back
            Homebrew.IO.File.Copy(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_MOConfig.xml", @"\My Documents\Zune\PimentoCache\Keepers\LKG_MOConfig.xml");
            Homebrew.IO.File.Copy(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_MOStoreConfig.xml", @"\My Documents\Zune\PimentoCache\Keepers\LKG_MOStoreConfig.xml");
            Homebrew.IO.File.Copy(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_OEMStoreConfig.xml", @"\My Documents\Zune\PimentoCache\Keepers\LKG_OEMStoreConfig.xml");
            Homebrew.IO.File.Copy(@"\My Documents\Zune\PimentoCache\Keepers\Backup\LKG_TunerConfig.xml", @"\My Documents\Zune\PimentoCache\Keepers\LKG_TunerConfig.xml");

            // Inform the user
            MessageBox.Show("Your original marketplace settings have been restored");
            throw new Exception("Exiting");
        }

        private void toggleAutoRegion_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.getInstance().saveValue("autoregion", toggleAutoRegion.IsChecked);
        }
    }
}