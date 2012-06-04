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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace Homebrew.Net.Sockets
{
    [ComImport, Guid(/*Mango*/"7A6562DA-6941-4DEF-8975-D55C9AC213A8"),
        
        // IF NODO "64422BB2-E864-4118-91A8-E969628DAFDF"),
      ClassInterface(ClassInterfaceType.None)]
    internal class NetworkClass
    {
    }

    [ComImport, Guid(/*Mango*/"68E7421C-F8A2-4041-9CF0-2BF5432C66FE"),
        
        // IF NODO "14F10380-E052-4033-945C-FBCC24417688"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface INetSockets
    {
        [PreserveSig]
        int Initialize(out int ret);
        [PreserveSig]
        int Cleanup(out int ret);
        [PreserveSig]
        int SendBytes(IntPtr socket, IntPtr buffer, int length, int flags, out int ret);
        [PreserveSig]
        int ReadBytes(IntPtr socket, IntPtr buffer, int length, out int ret);
        [PreserveSig]
        int Connect(string name, int port, out IntPtr socket, out int ret);
        [PreserveSig]
        int Close(IntPtr socket, out int ret);
        [PreserveSig]
        int Shutdown(IntPtr socket, out int ret);
        [PreserveSig]
        int SetRecieveTimeout(IntPtr socket, int timeout, out int ret);
        [PreserveSig]
        int SetSendTimeout(IntPtr socket, int timeout, out int ret);
        [PreserveSig]
        int Listen(int port, out IntPtr socket, out int ret);
        [PreserveSig]
        int Accept(IntPtr ListenSocket, out IntPtr socket, out sockaddr_in addr, ref int saddr, out int ret);
        [PreserveSig]
        int GetLastError(out int Error);
        [PreserveSig]
        int GetHostName( StringBuilder name, int len);
        [PreserveSig]
        int GetHostByNameLength(string name, out int len);
        [PreserveSig]
        int GetHostByNameItem(string name, int index, IntPtr buffer);

        [PreserveSig]
        int GetAdapterAddresses(AddressFamily Family, uint Flags, IntPtr pAdapterAddresses, ref uint pOutBufLen);
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal struct sockaddr_in
    {
        public const int Size = 16;

        public short sin_family;
        public ushort sin_port;
        public struct in_addr
        {
            public uint S_addr;
            public struct _S_un_b
            {
                public byte s_b1, s_b2, s_b3, s_b4;
            }
            public _S_un_b S_un_b;
            public struct _S_un_w
            {
                public ushort s_w1, s_w2;
            }
            public _S_un_w S_un_w;
        }
        public in_addr sin_addr;
    }

    internal static class SocketsApi
    {
        private static INetSockets m_netSocketInterface;

        static SocketsApi()
        {
            //IF NODO => InteropHelper.RegisterDLLOrExcept("NativeIO.dll", "64422BB2-E864-4118-91A8-E969628DAFDF");
            InteropHelper.RegisterDLLOrExcept("NativeIO_Mango.dll", "7A6562DA-6941-4DEF-8975-D55C9AC213A8");

            var nc = new NetworkClass();
            m_netSocketInterface = nc as INetSockets;
            int iret = Initialize();
            Debug.WriteLine("Winsock Initialized: " + iret);
        }

        public static int GetAdapterAddresses(AddressFamily family, uint flags, IntPtr pAdapterAddresses, ref uint pOutBufLen)
        {
            return m_netSocketInterface.GetAdapterAddresses(family, flags, pAdapterAddresses, ref pOutBufLen);
        }

        public static int Initialize()
        {
            int ret;
             m_netSocketInterface.Initialize(out ret);
             return ret;
        }

        public static int Cleanup()
        {
            int ret;
            m_netSocketInterface.Cleanup(out ret);
            return ret;
        }

        public static int SendBytes(IntPtr socket, byte[] buffer, int offset, int byteCount, int flags)
        {
            Debug.Assert(byteCount > 0);
            Debug.WriteLine("Sending " + byteCount + " bytes");
            int ret;
            var bufferHandle = Microsoft.Phone.InteropServices.GCHandle.Alloc(buffer, GCHandleType.Pinned);
            m_netSocketInterface.SendBytes(socket, new IntPtr(bufferHandle.AddrOfPinnedObject().ToInt32() + offset), byteCount, flags, out ret);
            Debug.WriteLine("Sent -> " + ret + " bytes");
            bufferHandle.Free();

            return ret;
        }

        public static int ReadBytes(IntPtr socket, byte[] buffer, int offset, int byteCount)
        {
            //Debug.Assert(buffer.Length >= offset + byteCount);

            Debug.Assert(buffer.Length >= (offset + byteCount));

            Debug.WriteLine("Read: buf=" + buffer.Length + " offset=" + offset + " count=" + byteCount);
           // System.Threading.Thread.Sleep(500);
            var bufferHandle = Microsoft.Phone.InteropServices.GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Debug.WriteLine("Reading " + byteCount + " from " + socket.ToInt32());
            int ret;
            m_netSocketInterface.ReadBytes(socket, new IntPtr(bufferHandle.AddrOfPinnedObject().ToInt32() + offset), byteCount, out ret);
            Debug.WriteLine(" --> Read " + ret);
           // System.Threading.Thread.Sleep(500);
            if (ret == SocketsApi.SOCKET_ERROR)
            {
               // Debug.WriteLine("There has been an error! ------------");
                Debug.WriteLine("Err: " + SocketsApi.GetLastError());
                
            }
            bufferHandle.Free();
          //  Debug.WriteLine("Handle Free");
            return ret;
        }

        public static int Connect(string name, int port, out IntPtr socket)
        {
            int ret;
             m_netSocketInterface.Connect(name, port, out socket, out ret);
             return ret;
        }

        public static int Listen(int port, out IntPtr socket)
        {
            int ret;
             m_netSocketInterface.Listen(port, out socket, out ret);
            return ret;
        }

        public static int Accept(IntPtr ListenSocket, out IntPtr socket, out string address, out int remote_port)
        {
            Debug.WriteLine("Waiting for socket accept");
            address = "Unknown";
            remote_port = 0;

            sockaddr_in sa;
            int sz = 16;
            int ret;
            m_netSocketInterface.Accept(ListenSocket, out socket, out sa, ref sz, out ret);
            if (ret == 0)
            {
                Debug.WriteLine("Accepted socket " + socket.ToInt32());
                remote_port = sa.sin_port;
                address = LongToIP((long)sa.sin_addr.S_addr);
            }
            else
            {
                Debug.WriteLine("FAILED Accepted socket " + ret);
            }
            return ret;
        }

        // ugh this code is ugly.
        static string LongToIP(long longIP)
        {
            string ip = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                int num = (int)(longIP / Math.Pow(256, (3 - i)));
                longIP = longIP - (long)(num * Math.Pow(256, (3 - i)));
                if (i == 0)
                    ip = num.ToString();
                else
                    ip = num.ToString() + "." + ip;
            }
            return ip;
        }

        public static int Close(IntPtr socket)
        {
            Debug.WriteLine("Closing socket " + socket.ToInt32());
            int ret, ret2;
            m_netSocketInterface.Shutdown(socket, out ret2);
             m_netSocketInterface.Close(socket, out ret);

            Debug.WriteLine("Closed Socket: " + ret + " Shutdown: " + ret2);
            if (ret2 == SOCKET_ERROR)
            {
                Debug.WriteLine("Error: " + GetLastError());
            }
            return ret;
        }

        public static int SetRecieveTimeout(IntPtr socket, int timeout)
        {
            Debug.WriteLine("Set receive timeout to " + timeout + " for " + socket.ToInt32());
            int ret;
            return m_netSocketInterface.SetRecieveTimeout(socket, timeout, out ret);
//            return ret;
        }

        public static int SetSendTimeout(IntPtr socket, int timeout)
        {
            Debug.WriteLine("Set send timeout to " + timeout + " for " + socket.ToInt32());
            int ret;
            return m_netSocketInterface.SetSendTimeout(socket, timeout, out ret);
//            return ret;
        }

        public static SocketError GetLastError()
        {
            int err;
            m_netSocketInterface.GetLastError(out err);

            return (SocketError)err;
        }

        public static string GetHostName()
        {
            StringBuilder sb = new StringBuilder(256);
            m_netSocketInterface.GetHostName( sb, 256);
            return sb.ToString();
        }

        public static int GetHostByNameLength(string host)
        {
            int len;
            m_netSocketInterface.GetHostByNameLength(host, out len);
            return len;
        }

        public static byte[] GetHostByNameItem(string host, int index)
        {
            byte[] buffer = new byte[32];

            var bufferHandle = Microsoft.Phone.InteropServices.GCHandle.Alloc(buffer, GCHandleType.Pinned);

            int ret =  m_netSocketInterface.GetHostByNameItem(host, index, bufferHandle.AddrOfPinnedObject());
            Debug.WriteLine("GetHostByNameItem RET: " + ret);
            bufferHandle.Free();
            return buffer;
        }

        public static readonly int SOCKET_ERROR = -1;
    }
}
