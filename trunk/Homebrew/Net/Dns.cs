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
using Microsoft.Phone.InteropServices;
using Homebrew.Net.Sockets;
using System.Diagnostics;

namespace Homebrew.Net
{
    public class Dns
    {
        public static string GetHostName()
        {
            return Sockets.SocketsApi.GetHostName();
        }

        public static IPHostEntry GetHostByName(string name)
        {
            int hosts = SocketsApi.GetHostByNameLength(name);
            Debug.WriteLine("Got host #" + hosts);

            for (int i = 0; i < hosts; i++)
            {
                byte[] addr = SocketsApi.GetHostByNameItem(name, i);
                string ret = "";
                foreach (byte x in addr)
                {
                    ret += " " + x.ToString();
                }
                Debug.WriteLine("Addr: " + ret);
            }

            return null;
        }
    }
}
