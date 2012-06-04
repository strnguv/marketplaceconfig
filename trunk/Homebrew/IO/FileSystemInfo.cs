using System;
using System.IO;
using Microsoft.Phone.InteropServices;
using System.Text;

using Homebrew.HtcRoot;

namespace Homebrew.IO
{
	public class FileAttributes
	{
		protected FileAttributesEnum _attr;

		public FileAttributes (uint flags)
			: this((FileAttributesEnum)flags)
		{}

		public FileAttributes (FileAttributesEnum flags)
		{
			_attr = flags;
		}

		public bool Normal { get { return (_attr & FileAttributesEnum.Normal) == FileAttributesEnum.Normal; } }

		public bool Directory { get { return (_attr & FileAttributesEnum.Directory) == FileAttributesEnum.Directory; } }

		public bool Archive { get { return (_attr & FileAttributesEnum.Archive) == FileAttributesEnum.Archive; } }

		public bool Hidden { get { return (_attr & FileAttributesEnum.Hidden) == FileAttributesEnum.Hidden; } }

		public bool InRom { get { return (_attr & FileAttributesEnum.InRom) == FileAttributesEnum.InRom; } }

		public bool System { get { return (_attr & FileAttributesEnum.System) == FileAttributesEnum.System; } }

		public bool ReadOnly { get { return (_attr & FileAttributesEnum.Readonly) == FileAttributesEnum.Readonly; } }

		public bool RomModule { get { return (_attr & FileAttributesEnum.RomModule) == FileAttributesEnum.RomModule; } }

		public FileAttributesEnum Flags { get { return _attr; } }

		public override String ToString ()
		{
			if (Normal) return String.Empty;
			char[] flags = { '\xA0', '\xA0', '\xA0', '\xA0', '\xA0', '\xA0', '\xA0' };
			if (Directory) flags[0] = 'D';
			if (Archive) flags[1] = 'A';
			if (System) flags[2] = 'S';
			if (Hidden) flags[3] = 'H';
			if (ReadOnly) flags[4] = 'R';
			if (InRom) flags[5] = 'I';
			if (RomModule) flags[6] = 'M';
			return new String(flags);
		}

		public String ToLongString ()
		{
			if (Normal) return "Normal";
			StringBuilder build = new StringBuilder();
			if (Directory) build.Append("Directory, ");
			if (Archive) build.Append("Archive, ");
			if (System) build.Append("System, ");
			if (Hidden) build.Append("Hidden, ");
			if (ReadOnly) build.Append("Read-only, ");
			if (InRom) build.Append("In ROM, ");
			if (RomModule) build.Append("ROM Module, ");
			if (build.Length > 2) build.Length -= 2;
			return build.ToString();
		}
	}

	public abstract class FileSystemInfo
	{
		internal FileAttributes _attr;
		internal WIN32_FIND_DATA _data;
		internal String _path, _full;
		internal DateTime? cTime, aTime, wTime;
		internal DirectoryInfo _parent;

		public FileSystemInfo (String path)
		{
			if (null == path) throw new ArgumentNullException("Trying to create FileSystemInfo for null path!");

			IntPtr handle = FileSystem.INVALID_HANDLE_VALUE;
			_path = Path.GetDirectoryName(path);
			if (null == _path) _path = String.Empty;
			int err = FileSystem.FindFirstFile(path, out _data, out handle);

			// The root doesn't get a proper name. Help it out
			if (String.IsNullOrWhiteSpace(_data.cFileName)) _data.cFileName = path;

			if (handle != FileSystem.INVALID_HANDLE_VALUE) FileSystem.FindClose(handle);

			if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == err)
			{
				throw new RootException("Unable to break out of sandbox to open path (" + path + ")!");
			}
			else if (0 != err)
			{
				throw new InteropException("Unable to open file system entity (" + path + ")! GetLastError: " + err + "\n");
			}
		}

		internal FileSystemInfo (String path, WIN32_FIND_DATA data)
		{
			_path = path;
			_data = data;
		}

		public DateTime CreationTime
		{
			get
			{
				if (!cTime.HasValue) cTime = (new DateTime(
					((long)(_data.ftCreationTime.dwHighDateTime) << 32) + _data.ftCreationTime.dwLowDateTime,
					DateTimeKind.Utc)
					.AddYears(1600));
				return cTime.Value;
			}
		}

		public DateTime CreationTimeUtc { get { return CreationTime; } }

		public DateTime LastAccessTime
		{
			get
			{
				if (!aTime.HasValue) aTime = (new DateTime(
					((long)(_data.ftLastAccessTime.dwHighDateTime) << 32) + _data.ftLastAccessTime.dwLowDateTime,
					DateTimeKind.Utc)
					.AddYears(1600));
				return aTime.Value;
			}
		}

		public DateTime LastAccessTimeUtc { get { return LastAccessTime; } }

		public DateTime LastWriteTime
		{
			get
			{
				if (!wTime.HasValue) wTime = (new DateTime(
					((long)(_data.ftLastWriteTime.dwHighDateTime) << 32) + _data.ftLastWriteTime.dwLowDateTime,
					DateTimeKind.Utc)
					.AddYears(1600));
				return wTime.Value;
			}
		}

		public DateTime LastWriteTimeUtc { get { return LastWriteTime; } }

		public String Name
		{
			get
			{
				return _data.cFileName;
			}
		}

		public String FullName
		{
			get
			{
				if (null == _full) _full = Path.Combine(_path, Name);
				return _full;
			}
		}

		public FileAttributes Attributes
		{
			get
			{
				if (null == _attr) _attr = new FileAttributes(_data.dwFileAttributes);
				return _attr;
			}
		}

	}
}
