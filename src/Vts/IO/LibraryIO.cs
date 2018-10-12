using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Vts.IO
{
    /// <summary>
    /// This class includes methods for dynamically loading DLLs
    /// </summary>
    public static class LibraryIO
    {
        private static IDictionary<string, string> _loadedAssemblies;
        // Location of the DLL
        private static string _dllLocation = "";

        static LibraryIO()
        {
            _loadedAssemblies = new Dictionary<string, string>();
        }

        /// <summary>
        /// Static method to check that an assemly is loaded
        /// </summary>
        /// <param name="assemblyName">Name of the assembly</param>
        public static void EnsureDllIsLoaded(string assemblyName)
        {
            if (!_loadedAssemblies.ContainsKey(assemblyName))
            {
                LoadFromDLL(_dllLocation + assemblyName);
            }
        }

        /// <summary>
        /// Loads an assembly from a dll
        /// </summary>
        /// <param name="fileName">Path and name of the dll</param>
        private static void LoadFromDLL(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            var assembly = Assembly.Load(bytes);
            _loadedAssemblies.Add(fileName, assembly.FullName);
        }
    }
}