using System;
using System.IO;
using System.Reflection;

namespace CreateFloor.Command
{
    internal class CreateFloorAssembly
    {
        public static string GetLocation()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uriBuilder = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uriBuilder.Path);
            return Path.GetDirectoryName(path);
        }

        public static string[] GetAssemblies()
        {
            return Directory.GetFiles(GetLocation(), "*.dll");
        }
    }
}
