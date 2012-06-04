using System.Collections.Generic;

namespace Homebrew.DeviceAPI.Registry
{
    public interface IRegistry
    {
        IEnumerable<string> GetSubKeys(string root, string path);
        IEnumerable<string> GetSubValuesNames(string root, string path);
        string GetValueType(string root, string path, string name);
        uint GetDwordValue(string root, string path, string name);
        string GetStringValue(string root, string path, string name);
        void SetDwordValue(string root, string path, string name, uint value);
        void SetStringValue(string root, string path, string name, string value);
    }
}
