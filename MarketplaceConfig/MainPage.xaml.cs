﻿using System;
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
using WP7RootToolsSDK;

namespace MarketplaceConfig
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Lists for the UI to data bind to
        ObservableCollection<Store> oems = new ObservableCollection<Store>();
        ObservableCollection<MOStore> mos = new ObservableCollection<MOStore>();

        // Message queue
        Queue<String> messageQueue = new Queue<string>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Test for unlock status
            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                // Disable everything since nothing will work right anyway
                listMO.IsEnabled = false;
                listOEM.IsEnabled = false;
                sliderMaxDownload.IsEnabled = false;

                // Inform the user
                messageQueue.Enqueue("This app requires root access. Please mark this app as 'Trusted' in WP7 Root Tools and restart the app.");
                return;
            }

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
        }

        // TODO: Clean up this method. It's a mess and hacky.
        private void populateLists()
        {
            try
            {
                listOEM.IsEnabled = false;
                listMO.IsEnabled = false;

                // This version of the silverlight toolkit is very picky. You can't set
                // the listpicker to an empty collection without it throwing exceptions,
                // so set them to 'non-empty' collection while prepping the real one
                List<Store> empty = new List<Store>();
                empty.Add(new Store("Error loading list", "", "", false, 
                    new Uri("http://winpho.foxingworth.com/marketplaceconfig/error.png"), "en-us"));
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

                if (oems.Count() > 0)
                    listOEM.ItemsSource = oems;

                if (mos.Count() > 0)
                    listMO.ItemsSource = mos;

                if (selectedOEM != null)
                    listOEM.SelectedItem = selectedOEM;
                if (selectedMO != null)
                    listMO.SelectedItem = selectedMO;

                listOEM.IsEnabled = true;
                listMO.IsEnabled = true;

                // This slider seems so easy in comparison :P
                sliderMaxDownload.Value = MarketWorker.getMaxDownload();

                textRestart.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                errorPrompt(ex);
            }
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
            catch (Exception ex)
            {
                errorPrompt(ex);
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
            }
            catch (InvalidCastException)
            {
                // This will happen when changing the lists
            }
            catch (Exception ex)
            {
                errorPrompt(ex);
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
                errorPrompt(ex);
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

        private void errorPrompt(Exception ex)
        {
            if (MessageBox.Show("The following error occured:\n\n" + ex.Message +
                "\n\nWould you like to submit this to the developer?", "Error",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                EmailComposeTask email = new EmailComposeTask();
                email.To = "trent@foxingworth.com";
                email.Subject = "Error report";
                email.Body = ex.Message + "\n" + ex.StackTrace;
                email.Show();
            }
        }
    }
}