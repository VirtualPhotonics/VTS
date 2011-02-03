using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Runtime.Serialization;
using Vts.Extensions;


namespace Vts.IO
{
    /// <summary>
    /// This class includes methods for dynamically loading DLLs
    /// </summary>
    public static class LibraryIO
    {
#if SILVERLIGHT
        private static Assembly SLDLL;
        private static string SLProjectName;

        /// <summary>
        /// Loads an assembly from a dll
        /// </summary>
        /// <param name="pathName">path name and filename of the dll</param>
        /// <param name="projectName">Type name of the instance (Project name)</param>
        public static void LoadFromDLL(string pathName, string projectName)
        {
            WebClient downloader = new WebClient(); 
            string path = pathName;
            SLProjectName = projectName;
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(downloader_OpenReadCompleted); 
            downloader.OpenReadAsync(new Uri(path, UriKind.Absolute));
        }

        static void downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            AssemblyPart assemblyPart = new AssemblyPart();
            SLDLL = assemblyPart.Load(e.Result);
            // SLDLL.CreateInstance(SLProjectName); //what is the scope of this instance?
        }
#else
        /// <summary>
        /// Loads an assembly from a dll
        /// </summary>
        /// <param name="filename">Path and name of the dll</param>
        /// <param name="projectName">Type name of the instance (Project Name)</param>
        /// <returns></returns>
        public static object LoadFromDLL(string filename, string projectName)
        {
            byte[] bytes = File.ReadAllBytes(filename);
            Assembly LoadedDLL = Assembly.Load(bytes);
            return LoadedDLL.CreateInstance(projectName);
        }
#endif
    }
}