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
using Homebrew.Net.Sockets;

namespace Homebrew.Net
{
    
    public abstract class EndPoint
    {
        protected EndPoint()
        {
        }

        public virtual EndPoint Create(SocketAddress socketAddress)
        {
            throw new NotImplementedException();
        }

        public virtual SocketAddress Serialize()
        {
            throw new NotImplementedException();
        }

        public virtual AddressFamily AddressFamily
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
     
}
