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

namespace Homebrew.Net.Sockets
{
    public class TcpClient : IDisposable
    {
        private Socket _socket = null;
        private NetworkStream _stream = null;

        public Socket Client { get { return _socket; } }

        public void Connect(string host, int port)
        {
            IntPtr socket;

            int ret = SocketsApi.Connect(host, port, out socket);

            if (ret != 0)
                throw new InteropException("Could not connect socket");

            _socket = new Socket(socket);
        }

        public TcpClient() { }

        internal TcpClient(Socket acceptedSocket)
        {
            _socket = acceptedSocket;
        }

        public NetworkStream GetStream()
        {
            if (_stream == null)
            {
                _stream = new NetworkStream(_socket);
            }
            return _stream;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                _socket.Close();
            }
        }

    }
}
