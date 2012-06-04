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
    public class TcpListener
    {
        public bool Active { get; private set; }
        public Socket Server { get; private set; }


        private IPAddress Address;
        private int Port;

        public TcpListener(int Port)
        {
            this.Port = Port;
            Server = null;
            Active = false;
        }

        public TcpListener(IPAddress Address, int Port)
        {
            // TODO: Complete member initialization
            this.Address = Address;
            this.Port = Port;
        }

        public void Start()
        {
            IntPtr socket;
            int ret = SocketsApi.Listen(Port, out socket);
            // TODO WSAError
            if (ret != 0) throw new InteropException("Unable to listen, Error: " + ret);
            Server = new Socket(socket);
            Server.IsBound = true;
        }

        public void Stop()
        {
            if (Server == null) throw new InvalidOperationException("Not listening");
            Server.Close();
            Server.IsBound = false;
        }

        public Socket AcceptSocket()
        {
            if (Server == null) throw new InvalidOperationException("Not listening");

            IntPtr client;
            string addr;
            int remote_port;
            int ret = SocketsApi.Accept(Server.InteralSocket, out client, out addr, out remote_port);
            // TODO WSAError
            if (ret != 0) throw new InteropException("Unable to accept client, Error: " + ret);

            Socket retSocket = new Socket(client);
            retSocket.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(addr), remote_port);
            //retSocket.IPAddress = addr;
            // TODO Remote Port, IPEndPoint
            return retSocket;
        }

        public TcpClient AcceptTcpClient()
        {
            return new TcpClient(AcceptSocket());
        }
        
        public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
        {
            return new TestAsyncResult<Socket>(() => AcceptSocket(), callback, state); 
        }

        public Socket EndAcceptSocket(IAsyncResult result)
        {
            var customAsync = result as TestAsyncResult<Socket>;
            if (customAsync == null)
            {
                throw new ArgumentException();
            }
            return customAsync.FetchResultsFromAsyncOperation(); 
        }

        public void Start(int backLog)
        {
            // TODO backLog
            Start();
        }

        public EndPoint LocalEndpoint
        {
            get
            {
                // TODO LocalEndpoint
                return new IPEndPoint(IPAddress.Any, 0);
            }
        }
    }
}
