using System.Collections.Generic;
#if SILVERLIGHT
using System;
using System.Net;
using System.Windows;
using System.Threading;
#else
using System.Reflection;
using System.IO;
#endif

namespace Vts.IO
{
    /// <summary>
    /// This class includes methods for dynamically loading DLLs
    /// </summary>
    public static class LibraryIO
    {
        private static IDictionary<string, string> _loadedAssemblies;
        // Location of the DLL
        private static string _dllLocation =
#if SILVERLIGHT
            "http://localhost:50789/Libraries/";
#else
            "";
#endif
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

#if SILVERLIGHT
        // a lightweight object to use 
        private static AutoResetEvent _signal = new AutoResetEvent(false);
        /// <summary>
        /// Loads an assembly from a dll
        /// </summary>
        /// <param name="fileName">path name and filename of the dll</param>
        private static void LoadFromDLL(string fileName)
        {
            WebClient downloader = new WebClient();

            downloader.OpenReadCompleted += (sender1, e1) =>
            {
                AssemblyPart assemblyPart = new AssemblyPart();
                var assembly = assemblyPart.Load(e1.Result);
                //Add the current assembly to the list of assemblies
                _loadedAssemblies.Add(fileName, assembly.FullName);
                _signal.Set();
            };

            downloader.OpenReadAsync(new Uri(fileName, UriKind.Absolute));

            // wait for the async operation to complete (-1 specifies an infinte wait time)
            _signal.WaitOne(-1);
        }
#else
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
#endif
    }
}