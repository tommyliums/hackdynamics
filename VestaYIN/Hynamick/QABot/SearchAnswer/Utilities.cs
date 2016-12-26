using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hynamick.SearchAnswer
{
    public sealed class Utilities
    {
        public static readonly string CurrentDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

        public static string GetFilePath(string relativePath)
        {
            return Path.Combine(CurrentDirectory, relativePath);
        }
    }
}
