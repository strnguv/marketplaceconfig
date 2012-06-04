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
using Homebrew.DeviceAPI;

namespace Homebrew
{
    public class Compass
    {
        private static ICompass _Compass = null;
        internal static ICompass DeviceCompass
        {
            get
            {
                if (_Compass == null)
                {
                    switch (Device.Hardware)
                    {
                        case Device.DeviceKind.Samsung:
                            _Compass = SamsungInterop.GetCompassInterface();
                            break;
                        default:
                            throw new NotSupportedException("The compass driver is only supported on Samsung Windows Phones");

                    }
                }

                return _Compass;
            }
        }

        public static int GetHeading()
        {
            return DeviceCompass.GetHeading();
        }


        public static bool IsSupported
        {
            get
            {
                try
                {
                    return DeviceCompass != null;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }
        }
    }

    internal interface ICompass
    {
        int GetHeading();
    }
}
