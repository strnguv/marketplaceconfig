using System;
using System.IO;
using Microsoft.Phone.InteropServices;

using Homebrew.HtcRoot;

namespace Homebrew.IO
{
    public class File
    {
        private IntPtr m_hFile;
        private FileAccess m_fileAccess;
        private long m_length;

		#region INTERNAL
		internal File()
        {

        }

		~File ()
		{
			if (Handle != IntPtr.Zero && Handle != FileSystem.INVALID_HANDLE_VALUE)
			{
				Close();
			}
		}

        internal int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            int ret = FileSystem.ReadFile(Handle, buffer, count, offset, out bytesRead);

            return bytesRead;
        }

		internal int Write (byte[] buffer, uint offset, uint count)
		{
			uint written = 0;
			int ret = FileSystem.WriteFile((UIntPtr)(int)Handle, buffer, count, offset, out written);
			return ret;
		}

        internal long Seek(long offset, SeekOrigin origin)
        {
            MoveMethod method;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    method = MoveMethod.Begin;
                    break;
                case SeekOrigin.Current:
                    method = MoveMethod.Current;
                    break;
                case SeekOrigin.End:
                    method = MoveMethod.End;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }

            return FileSystem.SeekFile(Handle, (int)offset, method);
        }

        internal void Close()
        {
            FileSystem.CloseFile(m_hFile);
        }

        internal FileAccess FileAccess
        {
            get { return m_fileAccess; }
            private set { m_fileAccess = value; }
        }

        internal long Length
        {
            get { return m_length; }
            private set { m_length = value; }
        }

        internal long Position
        {
            get
            {
                return FileSystem.SeekFile(Handle, 0, MoveMethod.Current);
            }
        }

        internal IntPtr Handle
        {
            get { return m_hFile; }
            private set { m_hFile = value; }
        }
		#endregion

		#region PUBLIC STATIC
		public static FileStream Open(string lpFilename,
                                      FileAccess dwDesiredAccess,
                                      FileShare dwShareMode,
                                      CreationDisposition dwCreationDisposition)
        {
            var file = new File();
            file.FileAccess = dwDesiredAccess;
            var dwFlagsAndAttributes = FileAttributesEnum.Normal;

			int err = 0;

			for (int i = 0;
				(i < 100) && (IntPtr.Zero == file.Handle) && (0 == err);
				i++)
			{
				err = FileSystem.OpenFile(lpFilename,
									dwDesiredAccess,
									dwShareMode,
									dwCreationDisposition,
									dwFlagsAndAttributes,
									out file.m_hFile);
			}

			if (HtcRoot.HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == err)
			{
				throw new HtcRoot.RootException("CreateFile failed for " + lpFilename + " due to Least Privileged Chamber!");
			}
            if (IntPtr.Zero == file.Handle)
            {
                throw new InteropException("CreateFile returned NULL for " + lpFilename + "! GetLastError: " + err + "\n");
            }
            if (FileSystem.INVALID_HANDLE_VALUE == file.Handle)
            {
                throw new InteropException("CreateFile failed for " + lpFilename + "! GetLastError: " + err + "\n");
            }

            file.m_length = FileSystem.GetFileSize(file.Handle);

            return new FileStream(file);
        }

        public static byte[] ReadAllBytes(string filename)
        {
            using (FileStream fs = Open(filename, FileAccess.Read, FileShare.Read, CreationDisposition.OpenExisting))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    return br.ReadBytes((int)fs.Length);
                }
            }
        }

        public static string ReadAllText(string filename)
        {
            byte[] bytes = ReadAllBytes(filename);
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

		public static void WriteAllBytes (String path, byte[] bytes)
		{
			using (FileStream stream = Open(path, FileAccess.Write, FileShare.None, CreationDisposition.CreateAlways))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}

        public static int Copy(string sourceFileName, string destFileName, bool failIfExists = false)
        {
            return FileSystem.CopyFile(sourceFileName, destFileName, failIfExists);
        }

		public static void Delete (String filename)
		{
			int ret = FileSystem.DeleteFile(filename);
			switch (ret)
			{
				case 0:
					return;
				case 2:
					throw new FileNotFoundException("Attempting to delete file (" + filename + ") resulted in ERROR_FILE_NOT_FOUND");
				case 3:
					throw new DirectoryNotFoundException("Attempting to delete file (" + filename + ") resulted in ERROR_PATH_NOT_FOUND");
				case 5:
					throw new UnauthorizedAccessException("Attempting to delete file (" + filename + ") resulted in ERROR_ACCESS_DENIED");
				case 32:
				case 33:
					throw new IOException("Attempting to delete file (" + filename + ") failed (might be in use)");
				case HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER:
					throw new RootException("Attempting to delete file (" + filename + ") failed due to Least Privileged Chamber");
				default:
					throw new InteropException("Attempting to delete file (" + filename + ") failed with error code " + ret);
			}
		}

		public static FileAttributes GetAttributes (String path)
		{
			FileAttributesEnum attribs;
			int ret = FileSystem.GetFileAttributes(path, out attribs);
			if (0 == ret) return new FileAttributes(attribs);
			else if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == ret)
				throw new RootException("Unable to get attributes on \"" + path + "\" due to Least Privileged Chamber");
			else
				throw new InteropException("Attempting to get attributes (" + path + ") failed with error code " + ret);
		}

		public static void SetAttributes (String path, FileAttributes fileAttributes)
		{
			int ret = FileSystem.SetFileAttributes(path, fileAttributes.Flags);
			if (0 == ret)
				return;
			else if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == ret)
				throw new RootException("Unable to set attributes on \"" + path + "\" due to Least Privileged Chamber");
			else
				throw new InteropException("Attempting to set attributes (" + path + ", " + (uint)(fileAttributes.Flags) + ") failed with error code " + ret);
		}
		#endregion
	}
}
