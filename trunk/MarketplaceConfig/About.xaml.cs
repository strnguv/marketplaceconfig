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
using Microsoft.Phone.Tasks;

namespace MarketplaceConfig
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            WebClient news = new WebClient();

            news.DownloadStringCompleted += new DownloadStringCompletedEventHandler(news_DownloadStringCompleted);
            news.DownloadStringAsync(new Uri("http://winpho.foxingworth.com/marketplaceconfig/news.txt"));
        }

        void news_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                textNews.Text = e.Error.Message;
            else
                textNews.Text = e.Result;
        }

        private void buttonEmail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "trent@foxingworth.com";
            email.Show();
        }

        private void buttonPM_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask pm = new WebBrowserTask();
            pm.Uri = new Uri("http://forum.xda-developers.com/private.php?do=newpm&u=1977599");
            pm.Show();
        }
    }
}