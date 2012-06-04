using System.Linq;
using System.Diagnostics;

namespace Homebrew
{
    public class Device
    {
        public enum DeviceKind
        {
            Samsung, HTC, LG, Unknown, NotChecked,
        }

        private static DeviceKind _Hardware = DeviceKind.NotChecked;
        public static DeviceKind Hardware
        {
            get
            {
                if (_Hardware == DeviceKind.NotChecked)
                {
                    DeviceAPI.Registry.WindowsCERegistry ce = new DeviceAPI.Registry.WindowsCERegistry();

                    if (ce.GetSubKeys("HKLM", "Software").Contains("Samsung"))
                    {
                        _Hardware = DeviceKind.Samsung;
                    }
                    else if (ce.GetSubKeys("HKLM", "Software").Contains("LGE"))
                    {
                        _Hardware = DeviceKind.LG;
                    }
                    // I don't have an HTC device, maybe they have an entry in the registry we 
                    // can test for (ideal), but until then, we'll rely on loading up the COM interface
                    // and then checking for isHTC
                    else
                    {
                        _Hardware = DeviceKind.HTC;
                    }
                    /* TODO this doesn't work, returns false
                    else if (DeviceAPI.HTCInterop.SuccessfullyLoaded)
                    {
                        if (DeviceAPI.HTCInterop.IsHTCDevice())
                        {
                            _Hardware = DeviceKind.HTC;
                        }
                    }
                    */
                    // TODO add support here for Dell, ASUS, other MFGs
                    
                    if (_Hardware == DeviceKind.NotChecked)
                    {
                        _Hardware = DeviceKind.Unknown;
                    }
                    Debug.WriteLine("Hardware: " + _Hardware);
                }
                return _Hardware;
            }
        }
    }
}
