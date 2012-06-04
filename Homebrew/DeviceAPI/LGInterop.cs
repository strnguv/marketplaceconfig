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
using System.Text;

namespace Homebrew.DeviceAPI
{
    /// <summary>
    /// Requires WP7STIDriver.dll, NewSysInfoComDLL.dll
    /// </summary>
    internal class LGInterop : InteropBase
    {
        static LGInterop()
        {
            try
            {
                InteropHelper.RegisterDLLOrExcept("WP7STIDriver.dll", "36E202C1-BBEE-46BA-BBB7-899936D3DCC1");
                InteropHelper.RegisterDLLOrExcept("NewSysInfoComDLL.dll", "B3BDED41-7A61-4388-8123-5ED1B185F002");
              
                SuccessfullyLoaded = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (Debugger.IsAttached) Debugger.Break();
            }
        }

        internal static IMfgTest GetTestInterface()
        {
            return new MfgTest() as IMfgTest;
        }

        internal static IEGMInterface GetInfoInterface()
        {
            return new CLGEComSysteminfoComObject() as IEGMInterface;
        }

        [ComImport, ClassInterface(ClassInterfaceType.None), Guid("36E202C1-BBEE-46ba-BBB7-899936D3DCC1")]
        public class MfgTest
        {
        }

        [ComImport, Guid("6E9E3A37-AD27-4966-8EFB-DE033429FD46"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMTDAsyncResultCallback
        {
            void AsyncResultUpdate(string result);
        }

        [ComImport, Guid("9663810F-02BD-4611-958B-F1D805871922"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMfgTest
        {
            void BRCM_ON(out bool pRet);
            void BRCM_OFF(out bool pRet);
            void Test1();
            void Test2(out int result);
            void RegisterForAsyncResultUpdate(IMTDAsyncResultCallback callback);
            void UnregisterAsyncResultUpdate();
            void AsyncTest1();
            int ProcessConfigXML(string filename, out int errorcode);
            int QueryConfigXML(out string resultxml);
            int ChangeConfigXML(string modifiedxml);
        }


        [ComImport, Guid("B3BDED41-7A61-4388-8123-5ED1B185F002"), ClassInterface(ClassInterfaceType.None)]
        public class CLGEComSysteminfoComObject
        {
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("1BE148C3-F0A5-4b5e-965E-14EC1C27C20B")]
        public interface IEGMInterface
        {
            void GetBetteryLifePercent(out byte betteryLifePercent);
            void getMemoryInfo(out double StorageTotal, out double StorageUse, out double ProgramTatol, out double ProgramUse);
            void RegSetValueDWORD(out int in_Key, string in_SubKey, string in_Name, out uint in_Data, out int out_Errcode);
            void RegGetValueDWORD(out int in_Key, string in_SubKey, string in_Name, out uint out_Data, out int out_Errcode);
            void RegSetValueSZ(out int in_Key, string in_SubKey, string in_Name, string in_Data, out int out_Errcode);
            void RegGetValueSZ(out int in_Key, string in_Subkey, string in_Name, out uint in_Length, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder out_Data, out int out_Errcode);
            void RegGetLengthSZ(out int in_Key, string in_Subkey, string in_Name, out uint out_Length, out int out_Errcode);
            void SysInfoStatusGet(out int bResult, out int errcode);
            void BootMenuSettings(out int bootMenuValue, out int retValue, out int retCode);
            void BandSettingStatus(out int bResult, out int errcode);
            void SystemTimeSet(out _TIMESTRUCT timeInfo, out int errorcode);
            void SystemTimeGet(out _TIMESTRUCT timeInfo, out int errorcode);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct _TIMESTRUCT
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

    }
}
