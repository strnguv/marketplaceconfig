﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

namespace Homebrew.Extensions
{
    public static class EncodingExtensions
    {
        public static string GetString(this Encoding enc, byte[] bytes)
        {
            return enc.GetString(bytes, 0, bytes.Length);
        }
    }
}
