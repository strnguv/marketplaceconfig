using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Homebrew.Net.Sockets
{
    class SocketException : Exception
    {
        // TODO finish?
        public SocketException(SocketError err) : base(err.ToString()){}
    }
}
