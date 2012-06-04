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
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Homebrew.Net.Sockets
{
    public class NetworkStream : Stream
    {
        protected Socket Socket;
        private bool ownsSocket;
        private FileAccess access;

        public NetworkStream(Socket socket)
        {
            Socket = socket;
        }

        public NetworkStream(Socket socket, bool ownsSocket)
        {
            // TODO: Complete member initialization
            this.Socket = socket;
            this.ownsSocket = ownsSocket;
        }

        public NetworkStream(Socket socket, FileAccess access)
        {
            // TODO: Complete member initialization
            this.Socket = socket;
            this.access = access;
        }

        public NetworkStream(Socket socket, FileAccess access, bool ownsSocket)
        {
            // TODO: Complete member initialization
            this.Socket = socket;
            this.access = access;
            this.ownsSocket = ownsSocket;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int ret =  Socket.Recieve(buffer, offset, count);
            /*
            Debug.WriteLine("Getting bytes...");
            StringBuilder buf = new StringBuilder();
            for (int i = offset; i < ret; i++)
            {
                buf.Append(" ");
                buf.Append((int)buffer[i]);
            }
            Debug.WriteLine("Got bytes: " + buf.ToString());
            */
            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {

        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Socket.Send(buffer, offset, count);
        }
    }
}
