using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Homebrew.DeviceAPI
{
    /// <summary>
    /// Requires ComRegRw.dll (Registry), DMXMLCOM (Provisioning)
    /// </summary>
    internal class HTCInterop : InteropBase
    {

        static HTCInterop()
        {
            try
            {
                InteropHelper.RegisterDLLOrExcept("ComRegRw.dll", "020CF84D-5946-4C9B-A066-8C9EC348DEDD");
                InteropHelper.RegisterDLLOrExcept("DMXMLCOM.dll", "9B0F4E01-7172-4A75-86AC-7F27AA9DD02D");
                SuccessfullyLoaded = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (Debugger.IsAttached) Debugger.Break();
            }
        }

        internal static IRegRwInterface GetRegistryInterface()
        {
            return new CRegRwClass() as IRegRwInterface;
        }

        internal static IProvisonXML GetProvisionInterface()
        {
            return new CProvisonXML() as IProvisonXML;
        }

        // HTC Registry

        [ComImport, Guid("020CF84D-5946-4C9B-A066-8C9EC348DEDD"), ClassInterface(ClassInterfaceType.None)]
        public class CRegRwClass
        {
        }

        [ComImport, Guid("BB2013C7-A917-4D43-B918-62CD659521EE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IRegRwInterface
        {
            void RegistryGetDword(uint uKey, string strSubKey, string strValueName, out uint uData, out uint uResult);
            void RegistrySetDword(uint uKey, string strSubKey, string strValueName, uint uData, out uint uResult);
            void RegistryGetString(uint uKey, string strSubKey, string strValueName, StringBuilder strData, out uint uResult);
            void RegistrySetString(uint uKey, string strSubKey, string strValueName, string pszData, out uint uResult);
            void Init(out uint uResult);
            void Deinit(out uint uResult);
            void IsHTCDevice(out uint isHTCDevice);
            void GetVersionInfo(string szSearchKey, StringBuilder szRequestVersion, out uint uRet);
        }

        // HTC FileSystem

        [ComImport, ClassInterface(ClassInterfaceType.None), Guid("EEA7F43B-A32D-4767-9AE7-9E53DA197455")]
        public class CFileRwClass
        {
        }

        [ComImport, Guid("4E0377B5-68C0-4885-B63D-3D5240E665EE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IFileRwInterface
        {
            void IsFileExist(string strPath, out uint uFileType, out uint uLastErr);
            void GetFileSize(string strPath, out uint uFileSize, out uint uLastErr);
            void ReadFile(string strPath, ref byte byteBuffer, uint uBufLen, out uint uLastErr);
            void WriteFile(string strPath, ref byte byteBuffer, uint uBufLen, uint uOverwrite, out uint uLastErr);
            void CopyFile(string strTarget, string strSource, uint uOverwrite, out uint uLastErr);
            void DeleteFile(string strPath, out uint uLastErr);
            void GetAllFileCounts(string strPath, string strSearch, out uint uBufLen, out uint uLastErr);
            void GetAllFileNames(string strPath, string strSearch, ref byte byteBuffer, uint uBufLen, out uint uLastErr);
            void Init(out uint uResult);
            void Deinit(out uint uResult);
        }

        // HTC Provision

        [ComImport, Guid("9B0F4E01-7172-4A75-86AC-7F27AA9DD02D"), ClassInterface(ClassInterfaceType.None)]
        internal class CProvisonXML
        {
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0785D047-6960-462C-9C1D-7824D604C0A8")]
        internal interface IProvisonXML
        {
            long DMProvisionXML(StringBuilder strXML, long nLen);
            long DMTest(out long nLen);
            long DMResetDevice();
            long MultipleDMProvisionXML(int nLen1, string strXML1, int nLen2, string strXML2, int nLen3, string strXML3, int nLen4, string strXML4, int nLen5, string strXML5);
        }

        public static bool IsHTCDevice()
        {
            if (!SuccessfullyLoaded) return false;

            var iface = GetRegistryInterface();
            uint isHTCDevice = 0;
            iface.IsHTCDevice(out isHTCDevice);
            return isHTCDevice == 0xb9d548dc;
        }
    }
}
