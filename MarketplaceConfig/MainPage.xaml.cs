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
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;

namespace MarketplaceConfig
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Lists for the UI to data bind to
        ObservableCollection<Store> oems = new ObservableCollection<Store>();
        ObservableCollection<MOStore> mos = new ObservableCollection<MOStore>();
        List<Region> regions = null;

        // Message queue
        Queue<String> messageQueue = new Queue<string>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Update the lists
            populateLists();

            // Run a backup
            if (MarketWorker.createBackup())
                messageQueue.Enqueue("Your current configuration was backed up. If you ever need to restore it, " +
                    "you can do so from the settings.");

            // Listen for list changing events
            listOEM.SelectionChanged += new SelectionChangedEventHandler(listOEM_SelectionChanged);
            listMO.SelectionChanged += new SelectionChangedEventHandler(listMO_SelectionChanged);

            // Update if auto-update is enabled
            UpdateManager.getInstance().UpdateComplete += new EventHandler(MainPage_UpdateComplete);
            if ((bool)SettingsManager.getInstance().loadValue("autoupdate"))
                UpdateManager.getInstance().startUpdate();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Message boxes are now safe to show without risking killing the app
            while (messageQueue.Count > 0)
                MessageBox.Show(messageQueue.Dequeue());

            /*
            listRegion.IsEnabled = !(bool)SettingsManager.getInstance().loadValue("autoregion");

            // Always unlock region if no MO is selected
            if (string.IsNullOrEmpty(((MOStore)listMO.SelectedItem).storeID))
                listRegion.IsEnabled = true;
             */
        }

        // TODO: Clean up this method. It's a mess and hacky.
        private void populateLists()
        {
            listOEM.IsEnabled = false;
            listMO.IsEnabled = false;

            // This version of the silverlight toolkit is very picky. You can't set
            // the listpicker to an empty collection without it throwing exceptions,
            // so set them to 'non-empty' collection while prepping the real one
            List<object> empty = new List<object>();
            empty.Add(new object());
            listOEM.ItemsSource = empty;
            listMO.ItemsSource = empty;

            // Get current market settings (for help with sorting, filtering, selecting items, etc)
            string currentOEM = MarketWorker.getOEMStore();
            string currentMO = MarketWorker.getMOStore();
            string currentRegion = MarketWorker.getMarketRegion();

            // Filter the sort the store lists
            IEnumerable<Store> oemList = from s in (List<Store>)SettingsManager.getInstance().loadValue("oems")
                                         orderby s.storeID
                                         select s;
            IEnumerable<MOStore> moList;
            if ((bool)SettingsManager.getInstance().loadValue("hideforeign"))
                moList = from s in (List<Store>)SettingsManager.getInstance().loadValue("mos")
                         where s.storeLocale.Equals(currentRegion, StringComparison.InvariantCultureIgnoreCase)
                         orderby s.storeName
                         select new MOStore(s); // Create MOStore from Store so they bind correctly in UI (show region)
            else
                moList = from s in (List<Store>)SettingsManager.getInstance().loadValue("mos")
                         orderby s.storeName
                         select new MOStore(s);

            // Populate and update the UI
            Store selectedOEM = null;
            MOStore selectedMO = null;

            oems.Clear();
            foreach (Store store in oemList)
            {
                oems.Add(store);
                if (store.Equals(currentOEM))
                    selectedOEM = store;
            }

            mos.Clear();
            foreach (MOStore store in moList)
            {
                mos.Add(store);
                if (store.Equals(currentMO))
                    selectedMO = store;
            }

            listOEM.ItemsSource = oems;
            listMO.ItemsSource = mos;

            if (selectedOEM != null)
                listOEM.SelectedItem = selectedOEM;
            if (selectedMO != null)
                listMO.SelectedItem = selectedMO;

            listOEM.IsEnabled = true;
            listMO.IsEnabled = true;
            
            /* this feature is not ready yet, mess with it at your own caution
            regions = new List<Region>();
            foreach (string region in (List<string>)SettingsManager.getInstance().loadValue("regions"))
                regions.Add(new Region(region));
            listRegion.ItemsSource = from r in regions orderby r.EnglishName select r;
             */


            // This slider seems so easy in comparison :P
            sliderMaxDownload.Value = MarketWorker.getMaxDownload();

            textRestart.Visibility = System.Windows.Visibility.Collapsed;
        }

        void MainPage_UpdateComplete(object sender, EventArgs e)
        {
            populateLists();

            // If this is an unknown store, ask to submit it
            if (!UpdateManager.getInstance().knownStore(MarketWorker.getMOStore())
                && !(bool)SettingsManager.getInstance().loadValue("submitasked"))
            {
                SettingsManager.getInstance().saveValue("submitasked", true);
                if (MessageBox.Show(MarketWorker.getMOStore() + " is a new market not yet known by the developer. " +
                    "Would you care to submit it to improve this app?\n\n(You can review what will be sent before submitting)",
                    "Unknown market", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MOStore submit = MarketWorker.generateMOStore();
                    EmailComposeTask email = new EmailComposeTask();
                    email.To = "marketplaceconfig@foxingworth.com";
                    email.Subject = submit.storeID;
                    email.Body = Store.saveStore(submit.toStore());
                    email.Show();
                }
            }
        }

        private void sliderMaxDownload_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (textMaxDownload != null)
                textMaxDownload.Text = sliderMaxDownload.Value.ToString("0") + " MB";
        }

        private void listOEM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            try
            {
                Store selected = (Store)listOEM.SelectedItem;

                // If this is not the current marketplace, inform of a change
                if (!selected.storeID.Equals(MarketWorker.getOEMStore(), StringComparison.InvariantCultureIgnoreCase))
                    textRestart.Visibility = System.Windows.Visibility.Visible;
            }
            catch (InvalidCastException)
            {
                // This will happen when changing the lists
            }
        }

        private void listMO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            try
            {
                Store selected = (Store)listMO.SelectedItem;

                // If this is not the current marketplace, inform of a change
                if (!selected.storeID.Equals(MarketWorker.getMOStore(), StringComparison.InvariantCultureIgnoreCase))
                    textRestart.Visibility = System.Windows.Visibility.Visible;

                /*
                // If auto-region is enabled, change the region accordingly
                if ((bool)SettingsManager.getInstance().loadValue("autoregion"))
                    foreach (Region region in listRegion.ItemsSource)
                        if (region.ShortName.Equals(selected.Locale, StringComparison.InvariantCultureIgnoreCase))
                        {
                            listRegion.SelectedItem = region;
                            break;
                        }
                 */
            }
            catch (InvalidCastException)
            {
                // This will happen when changing the lists
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            // Disable the elements to prevent the user from messing about while things save
            listOEM.IsEnabled = false;
            listMO.IsEnabled = false;
            listRegion.IsEnabled = false;
            sliderMaxDownload.IsEnabled = false;
            ApplicationBar.IsVisible = false;

            try
            {
                MarketWorker.setOEMStore((Store)listOEM.SelectedItem);
                MarketWorker.setMOStore(new MOStore((Store)listMO.SelectedItem));
                //MarketWorker.setMarketRegion((Region)listRegion.SelectedItem);
                MarketWorker.setMaxDownload(sliderMaxDownload.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // An ugly way to 'close' the app, but it works
            throw new Exception("Exiting");
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            throw new Exception("Exiting");
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void menuEraseSettings_Click(object sender, EventArgs e)
        {
            SettingsManager.getInstance().destroySettings();
            populateLists();
            MessageBox.Show("All settings cleared.");
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            listOEM.IsEnabled = false;
            listMO.IsEnabled = false;
            UpdateManager.getInstance().startUpdate();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
    }
}