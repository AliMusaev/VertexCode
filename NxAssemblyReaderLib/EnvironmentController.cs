using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NxAssemblyReaderLib
{
    class EnvironmentController
    {
        public void NX1980Set()
        {
            string path = Environment.GetEnvironmentVariable("path");
            string nxVersion = GetNxVersion();
            path = $"C:\\Siemens ; C:\\Siemens\\{nxVersion} ; C:\\Siemens\\{nxVersion}\\NXBIN ; C:\\Siemens\\{nxVersion}\\NXBIN\\managed ; {path}";
            Environment.SetEnvironmentVariable("path", path);
        }
        private string GetNxVersion()
        {
            string path = Assembly.GetAssembly(typeof(Program)).Location;
            using (StreamReader sr = new StreamReader(Path.GetDirectoryName(path) + "\\EnvironmentConfig.txt"))
            {
                string line = sr.ReadLine();
                var splits = line.Split(' ');
                return splits[1];

            }
        }
    }
}
