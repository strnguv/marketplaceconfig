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
using Homebrew.DeviceAPI.Provision;

namespace Homebrew.DeviceAPI.Registry
{
    public class LGRegistry : WindowsCERegistry
    {
        LGInterop.IEGMInterface native;

        public LGRegistry() : base()
        {
            native = LGInterop.GetInfoInterface();
        }

        public override uint GetDwordValue(string root, string path, string name)
        {

            int num = 1;    // TODO dunno what the 'encoding' is
            int num2 = 2; // TODO this is HKLM, dunno what to do the HKCU
            uint ret = 0;

            native.RegGetValueDWORD(out num2, path, name, out ret, out num);
            return ret;
        }

        public override void SetDwordValue(string root, string path, string name, uint value)
        {
            int num = 1;
            int num2 = 2; // TODO HKLM only

            native.RegSetValueDWORD(out num2, path, name, out value, out num);
        }

        public override void SetStringValue(string root, string path, string name, string value)
        {
            int num = 1;
            int num2 = 2; // TODO HKLM only

            native.RegSetValueSZ(out num2, path, name, value, out num);
        }
    }
}
