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
using System.Text;

namespace Homebrew.DeviceAPI.Provision
{
    public class HTCProvision : IProvision
    {
        private static HTCInterop.IProvisonXML native;

        public HTCProvision()
        {
            native = HTCInterop.GetProvisionInterface();
        }

        public long ProvisionXML(string xmlcontent)
        {
            return native.DMProvisionXML(new StringBuilder(xmlcontent), xmlcontent.Length);
        }
    }
}
