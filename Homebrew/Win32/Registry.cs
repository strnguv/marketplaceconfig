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
using Homebrew.DeviceAPI.Registry;
using System.Diagnostics;

namespace Homebrew.Win32
{
    public static class Registry
    {
        private static IRegistry _DeviceRegistry = null;
        [Obsolete("This is a temporary API until the Microsoft.Win32.Registry API is complete")]
        public static IRegistry DeviceRegistry
        {
            get
            {
                if (_DeviceRegistry == null)
                {
                    switch (Device.Hardware)
                    {
                        case Device.DeviceKind.HTC:
                            _DeviceRegistry = new HTCRegistry();
                            break;
                        case Device.DeviceKind.LG:
                            _DeviceRegistry = new LGRegistry();
                            break;
                        case Device.DeviceKind.Samsung:
                            _DeviceRegistry = new SamsungRegistry();
                            break;
                        default:
                            Debug.WriteLine("Using default Windows CE registry");
                            _DeviceRegistry = new WindowsCERegistry();
                            break;
                    }
                }
                return _DeviceRegistry;
            }
        }



        /*  This conforms with Microsoft.Wind32.Registry
        public static readonly RegistryKey ClassesRoot;
        public static readonly RegistryKey CurrentUser;
        public static readonly RegistryKey LocalMachine;

        public static object GetValue(string keyName, string valueName, object defaultValue)
        {
            throw new NotImplementedException();
        }

        public static void SetValue(string keyName, string valueName, object value)
        {
            throw new NotImplementedException();
        }
        */
    }
}
