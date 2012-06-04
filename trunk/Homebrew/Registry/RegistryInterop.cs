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
/*
namespace Homebrew.Registry
{
    [ComImport, Guid("515CC5F4-F317-4157-9C0E-9DDA8FBB6A3F"),
    ClassInterface(ClassInterfaceType.None)]
    internal class RegistryClass { }

    [ComImport, Guid("7ABEB6BC-C0AE-4024-B56F-625AEE698D79"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRegistry
    {
        [PreserveSig]
        int RegOpenKey(RegistryHive key, string subkey, out IntPtr handle);
    }

    [Flags]
    public enum RegistryHive : uint
    {
        HKEY_CLASSES_ROOT = 0x80000000,
        HKEY_CURRENT_USER = 0x80000001,
        HKEY_LOCAL_MACHINE = 0x80000002,
        HKEY_USERS = 0x80000003,
    }


    static public class RegistryAPI
    {
        static IRegistry m_registry = null;

        static RegistryAPI()
        {
            InteropHelper.RegisterDLLOrExcept("NativeIO.dll", "515CC5F4-F317-4157-9C0E-9DDA8FBB6A3F");
            m_registry = new RegistryClass() as IRegistry;
        }

        public static int OpenKey(RegistryHive key, string subkey, out IntPtr handle)
        {
            return m_registry.RegOpenKey(key, subkey, out handle);
        }
    }
}
*/