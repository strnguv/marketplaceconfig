using System;
using System.Collections.Generic;

using Homebrew.HtcRoot;

namespace Homebrew.IO
{
    public class Directory
    {
		public static void Delete (String path)
		{
			int ret = FileSystem.RemoveDirectory(path);
			if (0 == ret)
				return;
			else if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == ret)
				throw new RootException("Removing directory (" + path + ") failed due to Least Privileged Chamber");
			else
				throw new InteropException("Attempting to remove directory (" + path + ") failed with error code " + ret);
		}

		public static void CreateDirectory (String path)
		{
			int ret = FileSystem.CreateDirectory(path);
			if (0 == ret)
				return;
			else if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == ret)
				throw new RootException("Creating directory (" + path + ") failed due to Least Privileged Chamber");
			else
				throw new InteropException("Attempting to create directory (" + path + ") failed with error code " + ret);
		}

        public static string[] GetFiles(string path, string searchString = "*")
        {
//            if (path == "/favicon.ico") /*Damn you browsers auto pollig for icon => INVALID_HANDLE_VALUE */ return new string[0];


            var fileList = new List<string>();

            IntPtr hFind = IntPtr.Zero;
            WIN32_FIND_DATA findData;

            string fullPathQuery = System.IO.Path.Combine(path, searchString);

            int ret = FileSystem.FindFirstFile(fullPathQuery, out findData, out hFind);

            if (FileSystem.INVALID_HANDLE_VALUE == hFind)
            {
                throw new InteropException("Error opening directory: " + ret);
            }

            if ((findData.dwFileAttributes & FileAttributesEnum.Directory) != FileAttributesEnum.Directory)
            {
                fileList.Add(findData.cFileName);
            }

            while ((ret = FileSystem.FindNextFile(hFind, out findData)) == 0)
            {	// Note: return value of COM FindNextFile has changed
				// It now returns 0 on success, or GetLastError otherwise
                if ((findData.dwFileAttributes & FileAttributesEnum.Directory) != FileAttributesEnum.Directory)
                {
                    fileList.Add(findData.cFileName);
                }
            }

            FileSystem.FindClose(hFind);

			if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == ret)
				throw new RootException("Unable to get files in \"" + path + "\" due to Least Privileged Chamber");
			if (ret != FileSystem.ERROR_NO_MORE_FILES)
				throw new InteropException("Error getting files in " + path + ": " + ret);
            return fileList.ToArray();
        }

        public static string[] GetDirectories(string path, string searchString = "*")
        {
            var fileList = new List<string>();

            IntPtr hFind = IntPtr.Zero;
            WIN32_FIND_DATA findData;

            string fullPathQuery = System.IO.Path.Combine(path, searchString);

            int ret = FileSystem.FindFirstFile(fullPathQuery, out findData, out hFind);

            if (FileSystem.INVALID_HANDLE_VALUE == hFind)
            {
                throw new InteropException("Error opening directory: " + ret);
            }

            if ((findData.dwFileAttributes & FileAttributesEnum.Directory) == FileAttributesEnum.Directory)
            {
                fileList.Add(findData.cFileName);
            }

			while ((ret = FileSystem.FindNextFile(hFind, out findData)) == 0)
            {
                if ((findData.dwFileAttributes & FileAttributesEnum.Directory) == FileAttributesEnum.Directory)
                {
                    fileList.Add(findData.cFileName);
                }
            }

            FileSystem.FindClose(hFind);

			if (HtcRootAccess.ERROR_LEAST_PRIVILEGED_CHAMBER == ret)
				throw new RootException("Unable to get directories in \"" + path + "\" due to Least Privileged Chamber");
			if (ret != FileSystem.ERROR_NO_MORE_FILES)
				throw new InteropException("Error getting directories in " + path + ": " + ret);
			return fileList.ToArray();
        }

    }
}
