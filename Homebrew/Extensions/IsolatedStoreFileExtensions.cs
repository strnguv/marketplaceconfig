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
using System.IO.IsolatedStorage;
using System.IO;

namespace Homebrew.Extensions
{
    public static class IsolatedStorageFileExtensions
    {
        public static byte[] ReadAllBytes(this IsolatedStorageFile file, string filename)
        {
            using (var fs = file.OpenFile(filename, System.IO.FileMode.Open))
            {
                using (BinaryReader bw = new BinaryReader(fs))
                {
                    return bw.ReadBytes((int)fs.Length);
                }
            }
        }

        public static void WriteAllBytes(this IsolatedStorageFile file, string filename, byte[] data)
        {
            using (var fs = file.OpenFile(filename, System.IO.FileMode.OpenOrCreate))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(data);
                }
            }
        }
    }
}
