using System;
using System.Reflection;

namespace CreateFloor.Command
{
    public static class AssemblyLoader
    {
        public static void Load()
        {
            foreach (var file in CreateFloorAssembly.GetAssemblies())
            {
                try
                {
                    Assembly.LoadFrom(
                        file);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }
    }
}
