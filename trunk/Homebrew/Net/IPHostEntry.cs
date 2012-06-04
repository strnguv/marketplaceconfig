using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Homebrew.Net
{
    public class IPHostEntry
    {
        public IPAddress[] AddressList { get; set; }
        public string HostName { get; set; }
    }
}
