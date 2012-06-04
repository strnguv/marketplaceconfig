using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Phone.InteropServices;

using Homebrew.HtcRoot;

namespace Homebrew.IO
{
	public class DirectoryInfo : FileSystemInfo
	{
		public DirectoryInfo (String path)
			: base(path)
		{}

		internal DirectoryInfo (String path, WIN32_FIND_DATA data)
			: base(path,data)
		{}

		public DirectoryInfo Parent
		{
			get
			{
				if (null == _parent && !String.IsNullOrWhiteSpace(_path))
				{
					_parent = new DirectoryInfo(_path);
				}
				return _parent;
			}
		}

		public FileInfo[] GetFiles ()
		{
			return GetFiles("*");
		}

		public FileInfo[] GetFiles (String filter)
		{
			List<FileInfo> list = new List<FileInfo>();
			WIN32_FIND_DATA tempData;
			IntPtr handle;
			String path = Path.Combine(FullName, filter);
			int err = FileSystem.FindFirstFile(path, out tempData, out handle);

			if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == err)
			{
				throw new RootException("Failed to break out of sandbox to list files (" + path + ")!");
			}
			else if (FileSystem.ERROR_NO_MORE_FILES == err)
			{
				return list.ToArray();
			}
			else if (FileSystem.INVALID_HANDLE_VALUE == handle)
			{
				throw new InteropException("Error listing files (" + path + ")! GetLastError:" + err + "\n");
			}

			if ((tempData.dwFileAttributes & FileAttributesEnum.Directory) != FileAttributesEnum.Directory)
			{	// It's not a directory, so we'll say it's a file
				list.Add(new FileInfo(FullName, tempData));
			}

			// Note the change to FileSystem.FindNextFile return value
			while (0 == (err = FileSystem.FindNextFile(handle, out tempData)) )
			{
				if ((tempData.dwFileAttributes & FileAttributesEnum.Directory) != FileAttributesEnum.Directory)
				{	// It's not a directory, so we'll say it's a file
					list.Add(new FileInfo(FullName, tempData));
				}
			}

			FileSystem.FindClose(handle);

			if (err != FileSystem.ERROR_NO_MORE_FILES)
			{
				throw new InteropException("Error getting files in " + FullName + ": " + err);
			}
			return list.ToArray();
		}

		public DirectoryInfo[] GetDirectories ()
		{
			return GetDirectories("*");
		}

		public DirectoryInfo[] GetDirectories (String filter)
		{
			List<DirectoryInfo> list = new List<DirectoryInfo>();
			WIN32_FIND_DATA tempData;
			IntPtr handle;
			String path = Path.Combine(FullName, filter);
			int err = FileSystem.FindFirstFile(path, out tempData, out handle);

			if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == err)
			{
				throw new RootException("Failed to break out of sandbox to list directories (" + path + ")!");
			}
			else if (FileSystem.ERROR_NO_MORE_FILES == err)
			{
				return list.ToArray();
			}
			else if (FileSystem.INVALID_HANDLE_VALUE == handle)
			{
				throw new InteropException("Error listing subdirectories (" + FullName + ")! GetLastError: " + err + "\n");
			}

			if ((tempData.dwFileAttributes & FileAttributesEnum.Directory) == FileAttributesEnum.Directory)
			{	// It's a directory, include it
				list.Add(new DirectoryInfo(FullName, tempData));
			}

			while (0 == (err = FileSystem.FindNextFile(handle, out tempData)))
			{
				if ((tempData.dwFileAttributes & FileAttributesEnum.Directory) == FileAttributesEnum.Directory)
				{
					list.Add(new DirectoryInfo(FullName, tempData));
				}
			}

			FileSystem.FindClose(handle);

			if (err != FileSystem.ERROR_NO_MORE_FILES)
			{
				throw new InteropException("Error getting subdirectories in " + FullName + ": " + err);
			}
			return list.ToArray();
		}
	}
}
