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

namespace Homebrew.DeviceAPI.Provision
{
    public class LGProvision : IProvision
    {
        LGInterop.IMfgTest native;

        public LGProvision()
        {
            native = LGInterop.GetTestInterface();
        }

        public long ProvisionXML(string xmlcontent)
        {
            return native.ChangeConfigXML(xmlcontent);
        }
    }
}
