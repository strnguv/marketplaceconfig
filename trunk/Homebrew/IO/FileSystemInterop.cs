using System;
using System.Runtime.InteropServices;

using GCHandle = Microsoft.Phone.InteropServices.GCHandle;

namespace Homebrew.IO
{
    [ComImport, Guid(/*Mango*/"B0E4E41C-BE1D-4BA2-B8CE-7D632EA1CA37"),
        // IF NODO "BD4D0C42-91D1-44C3-86B0-4447FDF82BCE"),
       ClassInterface(ClassInterfaceType.None)]
    internal class FileSystemClass { }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WIN32_FIND_DATA
    {
        public FileAttributesEnum dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
    }

    [ComImport, Guid(/*Mango*/"1C6D96A9-1400-437D-ACC8-9E4DE23EDBA9"),
        // IF NODO "47A34768-AA01-4365-BB55-386BBB0FBBF6"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileSystem
    {
        [PreserveSig]
        int OpenFile(string lpFilename, int dwDesiredAccess, int dwShareMode, int dwCreationDisposition, int dwFlagsAndAttributes, out IntPtr hFile);

        [PreserveSig]
        int ReadFile(IntPtr hfile, IntPtr lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead);

		[PreserveSig]
		int WriteFile (UIntPtr hFile, UIntPtr pBuffer, uint nNumberOfBytesToWrite, out uint pnNumberOfBytesWritten);

        [PreserveSig]
        int CloseFile(IntPtr hFile);

        [PreserveSig]
        int SeekFile(IntPtr hFile, int lDistanceToMove, ref int lpDistanceToMoveHigh, int dwMoveMethod);

        [PreserveSig]
        int GetFileSize(IntPtr hFile, out int lpFileSizeHigh);

        [PreserveSig]
        int CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);

        [PreserveSig]
        int FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData, out IntPtr hFind);

        [PreserveSig]
        int FindNextFile(IntPtr hFind, out WIN32_FIND_DATA lpFindFileData);

        [PreserveSig]
        int FindClose(IntPtr hFind);

		[PreserveSig]
		int DeleteFile (String szFileName);

		[PreserveSig]
		int CreateDirectory (String szPathName);

		[PreserveSig]
		int RemoveDirectory (String szPathName);

		[PreserveSig]
		int GetFileAttributes (String szFilename, out FileAttributesEnum dwFileAttributes);

		[PreserveSig]
		int SetFileAttributes (String szFilename, uint dwFileAttributes);
    }

    [Flags]
    public enum FileAccess : uint
    {
        /// <summary>
        /// Read file access
        /// </summary>
        Read = 0x80000000,
        /// <summary>
        /// Write file acecss
        /// </summary>
        Write = 0x40000000,
        /// <summary>
        /// Execute file access
        /// </summary>
        Execute = 0x20000000,
        /// <summary>
        /// 
        /// </summary>
        All = 0x10000000,
		ReadWrite = (Read|Write)
    }

    [Flags]
    public enum FileShare : uint
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// Enables subsequent open operations on an object to request read access. 
        /// Otherwise, other processes cannot open the object if they request read access. 
        /// If this flag is not specified, but the object has been opened for read access, the function fails.
        /// </summary>
        Read = 0x00000001,
        /// <summary>
        /// Enables subsequent open operations on an object to request write access. 
        /// Otherwise, other processes cannot open the object if they request write access. 
        /// If this flag is not specified, but the object has been opened for write access, the function fails.
        /// </summary>
        Write = 0x00000002,
		/// <summary>
		/// Enables subesquent open operations requesting read and/or write access.
		/// Same as (FileShare.Read | FileShare.Write).
		/// </summary>
		ReadWrite = (Read|Write),
        /// <summary>
        /// Enables subsequent open operations on an object to request delete access. 
        /// Otherwise, other processes cannot open the object if they request delete access.
        /// If this flag is not specified, but the object has been opened for delete access, the function fails.
        /// </summary>
        Delete = 0x00000004,
		/// <summary>
		/// Enables all subsequent open operations, regardless of requested access.
		/// Same as (FileShare.Read | FileShare.Write | FileShare.Delete).
		/// </summary>
		All = (Read|Write|Delete)
    }

    public enum CreationDisposition : uint
    {
        /// <summary>
        /// Creates a new file. The function fails if a specified file exists.
        /// </summary>
        New = 1,
        /// <summary>
        /// Creates a new file, always. 
        /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes, 
        /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
        /// </summary>
        CreateAlways = 2,
        /// <summary>
        /// Opens a file. The function fails if the file does not exist. 
        /// </summary>
        OpenExisting = 3,
        /// <summary>
        /// Opens a file, always. 
        /// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
        /// </summary>
        OpenAlways = 4,
        /// <summary>
        /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
        /// The calling process must open the file with the GENERIC_WRITE access right. 
        /// </summary>
        TruncateExisting = 5
    }

    [Flags]
    public enum FileAttributesEnum : uint
    {
        Readonly =			0x00000001,
        Hidden =			0x00000002,
        System =			0x00000004,
        Directory =			0x00000010,
        Archive =			0x00000020,
        InRom =				0x00000040,
        Normal =			0x00000080,
        Temporary =			0x00000100,
        SparseFile =		0x00000200,
		ModuleNotTrusted =	0x00000200,
        ReparsePoint =		0x00000400,
		ModuleNoDebug =		0x00000400,
        Compressed =		0x00000800,
        Offline =			0x00001000,
		RomStaticRef =		0x00001000,
        NotContentIndexed = 0x00002000,
		RomModule =			0x00002000,
        Encrypted =			0x00004000,
        WriteThrough =		0x80000000,
        Overlapped =		0x40000000,
        NoBuffering =		0x20000000,
        RandomAccess =		0x10000000,
        SequentialScan =	0x08000000,
        DeleteOnClose =		0x04000000,
        BackupSemantics =	0x02000000,
        PosixSemantics =	0x01000000,
        OpenReparsePoint =	0x00200000,
        OpenNoRecall =		0x00100000,
        FirstPipeInstance = 0x00080000
    }

    internal enum MoveMethod : uint
    {
        Begin = 0,
        Current = 1,
        End = 2
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal struct LARGE_INTEGER
    {
        [FieldOffset(0)]
        public Int64 QuadPart;
        [FieldOffset(0)]
        public Int32 LowPart;
        [FieldOffset(4)]
        public Int32 HighPart;
    }

    internal static class FileSystem
    {
        private static IFileSystem m_fileSystemIo;
        private const int INVALID_FILE_SIZE = unchecked((int)0xffffffff);
        private const int INVALID_SET_FILE_POINTER = -1;
        private const int ERROR_ACCESS_DENIED = unchecked((int)0x80000005);

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		public const int ERROR_NO_MORE_FILES = 18;

        static FileSystem()
        {
            // IF NODO => InteropHelper.RegisterDLLOrExcept("NativeIO.dll", "BD4D0C42-91D1-44C3-86B0-4447FDF82BCE");
            InteropHelper.RegisterDLLOrExcept("NativeIO_Mango.dll", "B0E4E41C-BE1D-4BA2-B8CE-7D632EA1CA37");
            var fc = new FileSystemClass();
            m_fileSystemIo = fc as IFileSystem;
        }

        public static int OpenFile(string lpFilename, FileAccess dwDesiredAccess, FileShare dwShareMode, CreationDisposition dwCreationDisposition, FileAttributesEnum dwFlagsAndAttributes, out IntPtr hFile)
        {
            int ret = m_fileSystemIo.OpenFile(lpFilename, (int)dwDesiredAccess, (int)dwShareMode, (int)dwCreationDisposition, (int)dwFlagsAndAttributes, out hFile);
            return ret;
        }

        public static int ReadFile(IntPtr hfile, byte[] buffer, int nNumberOfBytesToRead, int offset, out int lpNumberOfBytesRead)
        {
            GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            int ret = m_fileSystemIo.ReadFile(hfile, new IntPtr(bufferHandle.AddrOfPinnedObject().ToInt32() + offset),
                                              nNumberOfBytesToRead,
                                              out lpNumberOfBytesRead);
            bufferHandle.Free();

            if (ret != 0)
            {
				if (ret == ERROR_ACCESS_DENIED)
                {
                    throw new UnauthorizedAccessException("Access denied");
                }
            }
            return ret;
        }

		public static int WriteFile (UIntPtr hFile, byte[] buffer, uint bytesToWrite, uint offset, out uint numberBytesWritten)
		{
			GCHandle hBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			int ret = m_fileSystemIo.WriteFile(hFile, (UIntPtr)((uint)hBuffer.AddrOfPinnedObject() + offset), bytesToWrite, out numberBytesWritten);
			hBuffer.Free();
			return ret;
		}

        public static int CloseFile(IntPtr hFile)
        {
            return m_fileSystemIo.CloseFile(hFile);
        }

        public static int SeekFile(IntPtr hFile, long lDistanceToMove, MoveMethod dwMoveMethod)
        {
            var li = new LARGE_INTEGER();

            li.QuadPart = lDistanceToMove;
            var ret = m_fileSystemIo.SeekFile(hFile, li.LowPart, ref li.HighPart, (int)dwMoveMethod);

            if (ret == INVALID_SET_FILE_POINTER)
                throw new Exception("Invalid seek");

            return ret;
        }

        public static long GetFileSize(IntPtr hFile)
        {
            var li = new LARGE_INTEGER();
            li.LowPart = m_fileSystemIo.GetFileSize(hFile, out li.HighPart);

            if (li.LowPart == INVALID_FILE_SIZE)
                throw new System.IO.IOException("Invalid file size");

            return li.QuadPart;
        }

        public static int CopyFile(string sourceFilename, string destinationFilename, bool failIfExists)
        {
            int ret = m_fileSystemIo.CopyFile(sourceFilename, destinationFilename, failIfExists);

            if (ret == 0)
            {
                int err = Microsoft.Phone.InteropServices.Marshal.GetLastWin32Error();

                if (err == ERROR_ACCESS_DENIED)
                {
                    throw new UnauthorizedAccessException("Access denied");
                }
            }

            return ret;
        }

		/// <summary>
		/// Finds the first file system entity whose name matches, and gets a handle for subsequent matches.
		/// </summary>
		/// <param name="lpFileName">The path\name\filter string to match on</param>
		/// <param name="lpFindFileData">Receives the found entity's data</param>
		/// <param name="hFind">Receives a handle for subsequent searches, or INVALID_HANDLE_VALUE on failure</param>
		/// <returns>0 on success, GetLastError otherwise</returns>
        public static int FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData, out IntPtr hFind)
        {
            return m_fileSystemIo.FindFirstFile(lpFileName, out lpFindFileData, out hFind);
        }

		/// <summary>
		/// Find the next file system entity matching the search string.
		/// </summary>
		/// <param name="hFind">Handle from a successful call to FindFirstFile</param>
		/// <param name="lpFindFileData">Receives the found entity's data</param>
		/// <returns>0 on success, GetLastError otherwise, check against ERROR_NO_MORE_FILES</returns>
        public static int FindNextFile(IntPtr hFind, out WIN32_FIND_DATA lpFindFileData)
        {
            return m_fileSystemIo.FindNextFile(hFind, out lpFindFileData);
        }

        public static int FindClose(IntPtr hFind)
        {
            return m_fileSystemIo.FindClose(hFind);
        }

		public static int DeleteFile (String szFileName)
		{
			int ret = m_fileSystemIo.DeleteFile(szFileName);
			// TODO: Identify some common exceptions here
			return ret;
		}

		public static int CreateDirectory (String szPathName)
		{
			int ret = m_fileSystemIo.CreateDirectory(szPathName);
			return ret;
		}

		public static int RemoveDirectory (String szPathName)
		{
			int ret = m_fileSystemIo.RemoveDirectory(szPathName);
			return ret;
		}

		public static int GetFileAttributes (String szFilename, out FileAttributesEnum dwFileAttributes)
		{
			return m_fileSystemIo.GetFileAttributes(szFilename, out dwFileAttributes);
		}

		public static int SetFileAttributes (String szFilename, FileAttributesEnum dwFileAttributes)
		{
			return m_fileSystemIo.SetFileAttributes(szFilename, (uint)dwFileAttributes);
		}
    }
}
