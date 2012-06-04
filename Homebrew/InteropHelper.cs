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
using System.Diagnostics;

namespace Homebrew
{
    public class InteropHelper
    {
        public static void RegisterDLLOrExcept(string dll, string guid)
        {
            uint ret = RegisterDLL(dll, guid);
            if (ret != 0)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                throw new InteropException(string.Format(
                    "Unable to register COM Library {0} ({1}) -> {2}", dll, guid, ret));
            }
        }

        public static uint RegisterDLL(string dll, string guid)
        {
            uint ret = ComBridge.RegisterComDll(dll, new Guid(guid));
            Debug.WriteLine(string.Format("COM Registration: {0} -> {1} ({2})", dll, ret, guid));
            return ret;
        }
    }

    public class InteropException : Exception
    {
        public InteropException(string msg) : base(msg) { }
    }
}
