using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Homebrew.DeviceAPI
{
    /// <summary>
    /// Requires FCRouterProxy.dll for Registry
    /// Requires COMRilClient.dll for Compass
    /// </summary>
    /// 

    internal class SamsungInterop : InteropBase
    {
        static SamsungInterop()
        {
            try
            {
                InteropHelper.RegisterDLLOrExcept("FCRouterProxy.dll", "BE2E2E71-FC72-4507-B8AD-A2FED536AAB0");
                InteropHelper.RegisterDLLOrExcept("COMRilClient.dll", "A18F6B1A-924E-4787-AA82-19F98B49CF5D");
                SuccessfullyLoaded = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (Debugger.IsAttached) Debugger.Break();
            }
        }

        internal static IHybridInterface_FCRProxy GetInterface()
        {
            return new CHybridClass_FCRProxy() as IHybridInterface_FCRProxy;
        }

        internal static ICompass GetCompassInterface()
        {
            return new SamsungCompass(GetInterface());
        }

        internal class SamsungCompass : ICompass
        {
            IHybridInterface_FCRProxy proxy;
            public SamsungCompass(IHybridInterface_FCRProxy proxy)
            {
                this.proxy = proxy;
            }

            public int GetHeading()
            {
                int bearing, hdst, pass, ret;
                
                proxy.Sensor_GetCompassBearing(out bearing, out hdst, out pass, out ret);
                Debug.WriteLine(bearing);
                return bearing;
            }
        }

                  
        [ComImport, ClassInterface(ClassInterfaceType.None), Guid("BE2E2E71-FC72-4507-B8AD-A2FED536AAB0")]
        public class CHybridClass_FCRProxy
        {
        }
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5F471A84-8D09-46fe-94D6-AF856A7CF3C8")]
        internal interface IHybridInterface_FCRProxy
        {
            void Error_GetLastErrorMessage(out string strErrorMessage);
            void Sensor_GetProximityValue(out int pdwObjectDetected, out int pdwReturn);
            void Sensor_GetAmbientLightValue(out int pdwMilliLux, out int pdwReturn);
            void Sensor_GetCompassValue(out float fHRawX, out float fHRawY, out float fHRawZ, out int pdwReturn);
            void Sensor_GetAccelerometerValue(out float pdwACCX, out float pdwACCY, out float pdwACCZ, out int pdwReturn);
            void FactoryProcess_GetHistoryNV(out int stringtext, out int pdwReturn);
            void FactoryProcess_GetResultNV(out int stringtext, out int pdwReturn);
            void USBSwitch_GetFunctionDriver(out uint nPathType, out int pdwReturn);
            void USBSwitch_SetFunctionDriver(uint nPathType, out int pdwReturn);
            void KeyTest_Enable(out int pdwReturn);
            void KeyTest_Disable(out int pdwReturn);
            void SMD_GetInfo(out string strSMDInfo, out int pdwReturn);
            void Camera_UpdateFirmwareVersion(out int pdwReturn);
            void Camera_GetFirmwareVersion(out string strISPFWVersion, out string strISPPRAVersion, out string strBINFWVersion, out string strBINPRAVersion, out string strFWVendorVersion, out string strFWReleaseVersion, out string strFWWriteCount, out int pdwReturn);
            void Camera_PerformFirmwareUpdate(int dwUpdateType, out int pdwReturn);
            void Camera_IsFirmwareUpdateValid(out bool bValid, out int pdwReturn);
            void Loopback_Test(int dwVoiceFlag, int dwLoopbackMode, int dwStartFlag, out int pdwReturn);
            void FactoryProcess_SetResultNV(int dwID, int dwPASS, out int pdwReturn);
            void HKLM_RegistryGetDWORD(string pszSubKey, string pszValueName, out int pdwData);
            void GetSubSystemConfig(out int dwReg0, out int dwData0, out int dwReg1, out int dwData1, out int dwReg2, out int dwData2, out int dwReg3, out int dwData3, out int dwReg4, out int dwData4, out int dwReg5, out int dwData5, out int dwReg6, out int dwData6, out int dwReg7, out int dwData7);
            void SetSubSystemConfig(int dwReg0, int dwData0, int dwReg1, int dwData1, int dwReg2, int dwData2, int dwReg3, int dwData3, int dwReg4, int dwData4, int dwReg5, int dwData5, int dwReg6, int dwData6, int dwReg7, int dwData7);
            void GetSystemGain(int reg0, out int reg1, out int reg2);
            void SetSystemGain(int reg0, int reg1, int reg2);
            void GetClassGain(int reg0, int reg1, out int reg2);
            void SetClassGain(int reg0, int reg1, int reg2);
            void GetDeviceGain(int reg0, out int reg1, out int reg2);
            void SetDeviceGain(int reg0, int reg1, int reg2);
            void SetDualMicControl(bool bOnOff);
            void Battery_GetLoggingStatus(out int pdwStatus, out int pdwReturn);
            void Battery_SetLoggingStatus(int dwEnable, out int pdwReturn);
            void FMRadio_Initialize(out bool bReturn);
            void FMRadio_UnInitialize(out bool bReturn);
            void FMRadio_GetStatusValues(out int nRSSI, out ushort unThreshold, out ushort unSNR, out ushort unStereoMode, out bool bReturn);
            void FMRadio_SetTheradholdValue(ushort unType, ushort unValue, out bool bReturn);
            void HKCU_RegistryGetDWORD(string pszSubKey, string pszValueName, out int pdwData, out int pdwReturn);
            void FMRadio_SetFrequency(int unFrequency, out bool pbReturn);
            void BT_GetMode(int dwGetFlag, int dwModeFlag, out int pdwStatus, out bool pbReturn);
            void System_Reboot(out bool pbReturn);
            void Sensor_GetProximityValue_Chip(out int pdwObjectDetected, out int pdwObjectValue, out int pdwReturn);
            void FMRadio_SetRegion(int dwRegion, out bool pbReturn);
            void Touch_RelianceTestEnable(out int pdwReturn);
            void Touch_RelianceTestDisable(out int pdwReturn);
            void Touch_GetDeltaValue(out int pdwXCoord, out int pdwYCoord, out int pdwDelta, out int pdwReference, out int pdwFirmwareVer, out int pdwReturn);
            void SetBacklightLevel(int nLevel);
            void GetFGInfo(out int Version, out int RCOMP, out int VCELL, out int rawSOC, out int adjSOC, out int pdwReturn);
            void SetFGQuickStart(out int pdwReturn);
            void SetLogEnable(out int pdwReturn);
            void GetPowerStatus(out int ACLineStatus, out int BatteryFlag, out int BatteryLifePercent, out int Reserved1, out int BatteryLifeTime, out int BatteryFullLifeTime, out int Reserved2, out int BackupBatteryFlag, out int BackupBatteryLifePercent, out int Reserved3, out int BackupBatteryLifeTime, out int BackupBatteryFullLifeTime, out int BatteryChemistry, out int BatteryVoltage, out int BatteryCurrent, out int BatteryAverageCurrent, out int BatteryAverageInterval, out int BatterymAHourConsumed, out int BatteryTemperature, out int BackupBatteryVoltage, out int pdwReturn);
            void GETADCInfo(out ushort VBATT, out ushort VCHG, out ushort INCHG, out ushort THERM, out bool TACABLE, out bool USBCABLE, out ushort BATT_ID, out int pdwReturn);
            void GetMVInfo(out short VBATT_MV, out short VCHG_MV, out short INCHG_OUT_MV, out short DEG, out short BAT_PER, out int pdwReturn);
            void SleepMode(out int pdwReturn);
            void Camera_SetSimpleTest(uint nPathType, out int pdwReturn);
            void SetPreventSleep(out int pdwReturn);
            void ReleasePreventSleep(out int pdwReturn);
            void FMRadio_ScanFrequency(out int nScanFrequnecy, out bool pbReturn);
            void Camera_GetSimpleTest(out uint nPathType, out int pdwReturn);
            void HKCU_RegistrySetDWORD(string pszSubKey, string pszValueName, uint dwData, out bool pbReturn);
            void HKLM_RegistrySetDWORD(string pszSubKey, string pszValueName, uint dwData, out bool pbReturn);
            void Touch_GetInfoValue(out int pdwFirmwareVer, out int pdwFirmwarebuild, out int pdwThreadHold, out int pdwXCoord, out int pdwYCoord, out int pdwReference1, out int pdwReference2, out int pdwReference3, out int pdwReference4, out int pdwReference5, out int pdwDelta1, out int pdwDelta2, out int pdwDelta3, out int pdwDelta4, out int pdwDelta5, out int pdwReturn);
            void TouchKey_GetInfoValue(out string pbstrKeyVer, out int pdwReturn);
            void FactoryProcess_GetDPPPVKKEY(out int pdwLength, out int stringtext, out int pdwReturn);
            void FactoryProcess_GetDPPPVKFILENAME(out int pdwLength, out int stringtext, out int pdwReturn);
            void RegistrySetString(int nKey, string pszSubKey, string pszValueName, string pszString, out bool pbReturn);
            void RegistryGetString(int nKey, string pszSubKey, string pszValueName, out string strKeyMessage, out bool pbReturn);
            void FMRadio_OutputSwitch(int OutputType, out bool pbReturn);
            void Light_SetCalValue(int dwValue, out int pdwReturn);
            void Accelerometer_SetCalValue(int dwValue, out int pdwReturn);
            void Sensor_GetCompassBearing(out int pdwBearing, out int pdwHDST, out int pdwPASS, out int pdwReturn);
            void FactoryProcess_SetDaylightTime(int dwTime, out int pdwReturn);
            void VolumeControl_waveOutSetVolume(int dwVolume, out int pdwReturn);
            void VolumeControl_waveOutGetVolume(out int dwVolume, out int pdwReturn);
            void DualMic_Test(int dwStartFlag, out int pdwReturn);
        }
    }

    /*
    public class SamsungInterop
    {
        public class SamsungDeviceException : Exception
        {
            public SamsungDeviceException(string message) : base(message) { }
        }

        static IHybridInterface_FCRProxy m_samsung;

        static SamsungInterop()
        {
            InteropHelper.RegisterDLLOrExcept("FCRouterProxy.dll", "BE2E2E71-FC72-4507-B8AD-A2FED536AAB0");
            m_samsung = new CHybridClass_FCRProxy() as IHybridInterface_FCRProxy;
        }

        public enum RegistryKey
        {
            CurrentUser = 1, LocalMachine = 0,
        }

        private static void ExceptOnError(bool success)
        {
            if (!success)
            {
                string str;
                m_samsung.Error_GetLastErrorMessage(out str);
                throw new SamsungDeviceException(str);
            }
        }

        public static void RegistryWrite(RegistryKey key, string subkey, string valueName, uint value)
        {
            bool success;
            if (key == RegistryKey.LocalMachine)
            {
                m_samsung.HKLM_RegistrySetDWORD(subkey, valueName, value, out success);
            }
            else if (key == RegistryKey.CurrentUser)
            {
                m_samsung.HKCU_RegistrySetDWORD(subkey, valueName, value, out success);
            }
            else
            {
                throw new NotImplementedException();
            }
            ExceptOnError(success);
        }

        public static void RegistryWrite(RegistryKey key, string subkey, string valueName, string value)
        {
            bool success;
            m_samsung.RegistrySetString((int)key, subkey, valueName, value, out success);
            ExceptOnError(success);
        }

        public static uint RegistryRead(RegistryKey key, string subkey, string valueName)
        {
            int value;
            if (key == RegistryKey.LocalMachine)
            {
                m_samsung.HKLM_RegistryGetDWORD(subkey, valueName, out value);
            }
            else if (key == RegistryKey.CurrentUser)
            {
                int ret;
                m_samsung.HKCU_RegistryGetDWORD(subkey, valueName, out value, out ret);
                // TODO check the value!
            }
            else
            {
                throw new NotImplementedException();
            }
            return (uint)value;
        }

        public static string RegistryReadStr(RegistryKey key, string subkey, string valueName)
        {
            string value;
            bool success;
            m_samsung.RegistryGetString((int)key, subkey, valueName, out value, out success);
            ExceptOnError(success);
            return value;
        }
    }
    */

}
