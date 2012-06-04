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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Homebrew.DeviceAPI
{
    /// <summary>
    /// Requires Native.dll
    /// </summary>

    internal class WindowsCEInterop
    {
        static WindowsCEInterop()
        {
            try
            {
                InteropHelper.RegisterDLLOrExcept("Native.dll", "C6BD09B4-96AA-4524-89C4-665A15DD7C9B");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (Debugger.IsAttached) Debugger.Break();
            }
        }

        internal static INativeRegistry GetRegistryInterface()
        {
            return new CNativeRegistry() as INativeRegistry;
        }

        [ComImport, Guid("C6BD09B4-96AA-4524-89C4-665A15DD7C9B"), ClassInterface(ClassInterfaceType.None)]
        internal class CNativeRegistry
        {
        }

        [ComImport, Guid("34744FD5-ED3B-4418-83E2-B931356F9A4D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface INativeRegistry
        {
            [PreserveSig]
            uint GetVersion([MarshalAs(UnmanagedType.LPWStr)] out string result);

            [PreserveSig]
            uint GetSubKeys(string root, string path, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [PreserveSig]
            uint GetValuesNames(string root, string path, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [PreserveSig]
            uint GetValueType(string root, string path, string name, out uint result);

            [PreserveSig]
            uint GetDwordValue(string root, string path, string name, out uint result);

            [PreserveSig]
            uint GetStringValue(string root, string path, string name, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [PreserveSig]
            uint GetMultiStringValue(string root, string path, string name, [MarshalAs(UnmanagedType.LPWStr)] out string result);

            [PreserveSig]
            uint SetDwordValue(string root, string path, string name, uint value);

            [PreserveSig]
            uint SetStringValue(string root, string path, string name, string value);
        }
    }
}
