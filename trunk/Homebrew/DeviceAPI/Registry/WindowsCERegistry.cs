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
using System.Collections.Generic;
using System.Linq;

namespace Homebrew.DeviceAPI.Registry
{
    public class WindowsCERegistry : IRegistry
    {
        internal const int ERROR_SUCCESS = 0;

        private const int REG_SZ = 1;
        private const int REG_DWORD = 4;
        private const int REG_MULTI_SZ = 7;

        private static WindowsCEInterop.INativeRegistry native;

        public WindowsCERegistry()
        {
            native = WindowsCEInterop.GetRegistryInterface();
        }

        public virtual IEnumerable<string> GetSubKeys(string root, string path)
        {
            string value = null;
            var hr = native.GetSubKeys(root, path, out value);
            if (hr != ERROR_SUCCESS)
            {
                if (hr == 1260) throw new UnauthorizedAccessException();
                if (hr == 2) throw new KeyNotFoundException();
                throw new ArgumentException("Error getting subkeys: " + hr);
            }

            if (value.Length > 0)
            {
                return value.Split('|').Where(s => s.Length > 0).OrderBy(s => s).ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        public virtual IEnumerable<string> GetSubValuesNames(string root, string path)
        {
            string value = null;
            var hr = native.GetValuesNames(root, path, out value);
            if (hr != ERROR_SUCCESS)
            {
                if (hr == 1260) throw new UnauthorizedAccessException();
                throw new ArgumentException("Error getting sub values: " + hr);
            }

            if (value.Length > 0)
            {
                return value.Split('|').Where(s => s.Length > 0).ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        public virtual string GetValueType(string root, string path, string name)
        {
            uint value = 0;
            var hr = native.GetValueType(root, path, name, out value);
            if (hr != ERROR_SUCCESS)
            {
                if (hr == 13) throw new ArgumentException("Invalid format.");
                if (hr == 2) throw new KeyNotFoundException();
                throw new ArgumentException("Error getting value type: " + hr);
            }

            string strvalue = "Unknown";
            switch (value)
            {
                case REG_SZ:
                    strvalue = "String";
                    break;
                case REG_MULTI_SZ:
                    strvalue = "Multi String";
                    break;
                case REG_DWORD:
                    strvalue = "Integer";
                    break;
                default:
                    strvalue = string.Format("Unknown ({0})", value);
                    break;
            }

            return strvalue;
        }

        public virtual uint GetDwordValue(string root, string path, string name)
        {
            uint value = 0;
            var hr = native.GetDwordValue(root, path, name, out value);
            if (hr != ERROR_SUCCESS)
            {
                if (hr == 2) throw new KeyNotFoundException();
                if (hr == 13) throw new ArgumentException("Invalid format.");
                if (hr == 1260) throw new UnauthorizedAccessException();
                throw new ArgumentException("Error getting int value: " + hr);
            }
            return value;
        }

        public virtual string GetStringValue(string root, string path, string name)
        {
            string value = null;
            var hr = native.GetStringValue(root, path, name, out value);
            if (hr != ERROR_SUCCESS)
            {
                if (hr == 2) throw new KeyNotFoundException();
                if (hr == 13) throw new ArgumentException("Invalid format.");
                if (hr == 1260) throw new UnauthorizedAccessException();
                throw new ArgumentException("Error getting string value: " + hr);
            }

            return value;
        }

        public virtual void SetDwordValue(string root, string path, string name, uint value)
        {
            var hr = native.SetDwordValue(root, path, name, value);
            if (hr != ERROR_SUCCESS)
            {
                if (hr == 2) throw new KeyNotFoundException();
                if (hr == 5 || hr == 1260) throw new UnauthorizedAccessException();
                if (hr == 13) throw new ArgumentException("Invalid format.");
                throw new ArgumentException("Error setting int value: " + hr);
            }
        }

        public virtual void SetStringValue(string root, string path, string name, string value)
        {
            var hr = native.SetStringValue(root, path, name, value);
            if (hr != ERROR_SUCCESS)
            {
                if (hr == 2) throw new KeyNotFoundException();
                if (hr == 5 || hr == 1260) throw new UnauthorizedAccessException("Acess denied");
                if (hr == 13) throw new ArgumentException("Invalid format.");
                throw new ArgumentException("Error setting string value: " + hr);
            }
        }
    }



}
