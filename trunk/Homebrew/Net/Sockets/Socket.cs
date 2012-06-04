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
using System.Diagnostics;

namespace Homebrew.Net.Sockets
{
    public class Socket : IDisposable
    {
        private IntPtr m_socket;
        private int m_sendTimeout = 20000;
        private int m_recieveTimeout = 20000;

 
        internal Socket(IntPtr _socket)
        {
            InteralSocket = _socket;
            Total = 0;
        }

        internal IntPtr InteralSocket
        {
            get { return m_socket; }
            set
            {
                m_socket = value;
               // RecieveTimeout = m_recieveTimeout;
               // SendTimeout = m_sendTimeout;

            }
        }

        public int Send(byte[] buffer, int offset, int byteCount)
        {
            if (InteralSocket == IntPtr.Zero)
            {
                return -1;
            }

            //Debug.Assert(InteralSocket != IntPtr.Zero); 
            if ((byteCount + offset) > buffer.Length)
                throw new ArgumentException("Length cannot be larger than the buffer", "byteCount");

            int ret = SocketsApi.SendBytes(InteralSocket, buffer, offset, byteCount, 0);
            SocketError err = SocketsApi.GetLastError();
            if (ret == SocketsApi.SOCKET_ERROR)
            {
                Connected = false;
                Debug.WriteLine("Send Error: " + err);
                Close();
            }
            if (ret == -1) return 0;
            return ret;
        }

        public int Recieve(byte[] buffer, int offset, int readByteCount)
        {
            Debug.Assert(InteralSocket != IntPtr.Zero); 
            if ((readByteCount + offset) > buffer.Length)
                throw new ArgumentException("Length cannot be larger than the buffer", "readByteCount");

            int ret = SocketsApi.ReadBytes(InteralSocket, buffer, offset, readByteCount);
            SocketError err = SocketsApi.GetLastError();
            if (ret == SocketsApi.SOCKET_ERROR && err != SocketError.Success)
            {
                Connected = false;
                Debug.WriteLine("Recieve Error: " + err);
                Close();
            }
            else if (ret == 0 && readByteCount != 0)
            {
                Connected = false;
                Debug.WriteLine("Socket closed gracefully");
                Close();
            }
            Total += ret;
            Debug.WriteLine("Total read: " + Total);

            if (ret == -1) return 0;
            return ret;
        }

        public int Recieve(byte[] buffer)
        {
            return Recieve(buffer, 0, buffer.Length);
        }

        public int SendTimeout
        {
            get { return m_sendTimeout; }
            set
            {
                m_sendTimeout = value;
                SocketsApi.SetSendTimeout(InteralSocket, m_sendTimeout);
            }
        }

        public int RecieveTimeout
        {
            get { return m_recieveTimeout; }
            set
            {
                m_recieveTimeout = value;
                SocketsApi.SetRecieveTimeout(InteralSocket, m_recieveTimeout);
            }
        }

        public void Close()
        {
            if (InteralSocket != IntPtr.Zero)
            {
                int ret = SocketsApi.Close(InteralSocket);
                InteralSocket = IntPtr.Zero;
                Debug.WriteLine("Closed: " + ret);
                Connected = false;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public bool IsBound { get; internal set; }

        public IPEndPoint RemoteEndPoint { get; internal set; }

        public void Disconnect(bool p)
        {
            Close();
        }

        public bool Connected { get; internal set; }

        public int Total { get; set; }
    }
}
