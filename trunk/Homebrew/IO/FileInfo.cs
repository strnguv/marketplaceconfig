using System;
using System.IO;
using System.Text;
using System.Net;

using Homebrew;

namespace Homebrew.IO
{
	/// <summary>
	/// Partial implementation of System.IO.FileInfo. Is probably a faster way to get the sizes of many files at once.
	/// </summary>
	public class FileInfo : FileSystemInfo
	{
		public FileInfo (String path)
			: base(path)
		{}

		internal FileInfo (String path, WIN32_FIND_DATA data)
			: base(path, data)
		{}

		protected String _lenStr = null;

		public DirectoryInfo Directory
		{
			get
			{
				if (null == _parent)
				{
					_parent = new DirectoryInfo(_path);
				}
				return _parent;
			}
		}

		public String DirectoryName { get { return _path; } }

		public long Length
		{
			get
			{
				return ((long)(_data.nFileSizeHigh) << 32) + _data.nFileSizeLow;
			}
		}

		public String LengthString
		{
			get
			{
				if (null == _lenStr)
				{
					long l = Length;
					_lenStr =
						((l >> 30) > 10L) ? (l >> 30) + "\xA0GB" :
						((l >> 20) > 10L) ? (l >> 20) + "\xA0MB" :
						(l > 10240L) ? (l >> 10) + "\xA0KB" :
						l + "\xA0\xA0" + "B";
				}
				return _lenStr;
			}
		}
	}
}
