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

namespace Homebrew.DeviceAPI.Registry
{
    public class SamsungRegistry : WindowsCERegistry
    {
        private static bool initialized = false;
        private static SamsungInterop.IHybridInterface_FCRProxy native;

        public SamsungRegistry() : base()
        {
            native = SamsungInterop.GetInterface();
        }

        const int HKLM = 0;
        const int HKCU = 1;
        private static int StringToKey(string key)
        {
            switch (key.ToUpper())
            {
                case "HKLM":
                case "HKEY_LOCAL_MACHINE":
                    return HKLM;
                case "HKCU":
                case "HKEY_CURRENT_USER":
                    return HKCU;
            }
            throw new ArgumentException("Wrong root");
        }

        public override void SetStringValue(string root, string path, string name, string value)
        {
            bool success;
            native.RegistrySetString(StringToKey(root), path, name, value, out success);

            if (!success)
            {
                string msg;
                native.Error_GetLastErrorMessage(out msg);
                throw new Exception(msg);
            }
        }

        public override void SetDwordValue(string root, string path, string name, uint value)
        {
            bool success;
            var key = StringToKey(root);

            if (key == HKLM)
            {
                native.HKLM_RegistrySetDWORD(path, name, value, out success);
            }
            else
            {
                native.HKCU_RegistrySetDWORD(path, name, value, out success);
            }

            if (!success)
            {
                string msg;
                native.Error_GetLastErrorMessage(out msg);
                throw new Exception(msg);
            }
        }
    }
}
