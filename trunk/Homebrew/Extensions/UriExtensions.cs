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

namespace Homebrew.Extensions
{
    public static class UriExtensions
    {
        public static string PathAndQuery(this Uri self)
        {
            if (!string.IsNullOrEmpty(self.Query))
            {
                return self.AbsolutePath + "?" + self.Query;
            }
            else
            {
                return self.AbsolutePath;
            }
        }
    }
}
