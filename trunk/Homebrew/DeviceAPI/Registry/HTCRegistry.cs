using System.Net;

namespace Homebrew.DeviceAPI.Registry
{
    public class HTCRegistry : WindowsCERegistry
    {
        private enum E_HKEY
        {
            E_HKEY_CLASSES_ROOT,
            E_HKEY_CURRENT_USER,
            E_HKEY_LOCAL_MACHINE,
            E_HKEY_USERS
        }

        private HTCInterop.IRegRwInterface native;
        private Provision.HTCProvision provision;

        public HTCRegistry() : base()
        {
            native = HTCInterop.GetRegistryInterface();
            provision = new Provision.HTCProvision();
        }

        internal bool IsHTCDevice()
        {
            uint isHTCDevice = 0;
            native.IsHTCDevice(out isHTCDevice);
            return isHTCDevice == 0xb9d548dc;
        }

        private bool Init()
        {
            uint uResult = 0;
            native.Init(out uResult);
            return uResult > 0;
        }

        private bool Deinit()
        {
            uint uResult = 0;
            native.Deinit(out uResult);
            return uResult > 0;
        }

        private E_HKEY StringToHKey(string key)
        {
            E_HKEY result = E_HKEY.E_HKEY_LOCAL_MACHINE;
            if (key == "HKLM" || key == "HKEY_LOCAL_MACHINE")
            {
                result = E_HKEY.E_HKEY_LOCAL_MACHINE;
            }
            else if (key == "HKCU" || key == "HKEY_CURRENT_USER")
            {
                result = E_HKEY.E_HKEY_CURRENT_USER;
            }
            else if (key == "HKEY_USERS")
            {
                result = E_HKEY.E_HKEY_USERS;
            }
            else if (key == "E_HKEY_CLASSES_ROOT")
            {
                result = E_HKEY.E_HKEY_CLASSES_ROOT;
            }

            return result;
        }

        public override void SetStringValue(string root, string path, string name, string value)
        {
            string format = @"<wap-provisioningdoc>
                                <characteristic type='Registry'>
                                    <characteristic type='{0}\{1}'>
                                        <parm name='{2}' value='{3}' datatype='string' />
                                    </characteristic>
                                </characteristic>
                             </wap-provisioningdoc>";

            var xml = string.Format(format, root, path, HttpUtility.HtmlEncode(name), HttpUtility.HtmlEncode(value));

            provision.ProvisionXML(xml);

            //var key = StringToHKey(root);
            //if (Init())
            //{
            //    uint hr = 0;
            //    native.RegistrySetString((uint)key, path, name, value, out hr);
            //    Deinit();

            //    if (hr == 0) throw new Exception(string.Format("Unknown error: {0}", hr));
            //}
            //else
            //{
            //    throw new Exception("HTC Driver Init failed");
            //}
        }

        public override void SetDwordValue(string root, string path, string name, uint value)
        {
            string format = @"<wap-provisioningdoc>
                                <characteristic type='Registry'>
                                    <characteristic type='{0}\{1}'>
                                        <parm name='{2}' value='{3}' datatype='integer' />
                                    </characteristic>
                                </characteristic>
                             </wap-provisioningdoc>";

            var xml = string.Format(format, root, path, HttpUtility.HtmlEncode(name), ((int)value).ToString());

            provision.ProvisionXML(xml);

            //var key = StringToHKey(root);
            //if (Init())
            //{
            //    uint hr = 0;
            //    native.RegistrySetDword((uint)key, path, name, value, out hr);
            //    Deinit();

            //    if (hr == 0) throw new Exception(string.Format("Unknown error: {0}", hr));
            //}
            //else
            //{
            //    throw new Exception("HTC Driver Init failed");
            //}
        }
    }
}
