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
        private static Assembly _sLDLL;
        //private static string _sLProjectName;
        private static List<string> _assemblyList = new List<string>();

#if SILVERLIGHT

        /// <summary>
        /// Loads an assembly from a dll
        /// </summary>
        /// <param name="FileName">path name and filename of the dll</param>
        public static void LoadFromDLL(string FileName)
        {
            WebClient downloader = new WebClient(); 
            string path = FileName.ToString();
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(downloader_OpenReadCompleted); 
            downloader.OpenReadAsync(new Uri(path, UriKind.Absolute));
        }

        public static void PollDLL(string AssemblyName)
        {
            bool IsLoaded;
            do
            {
                IsLoaded = IsDLLLoaded(AssemblyName.ToString());
            } while (!IsLoaded);
        }

        static void downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            AssemblyPart assemblyPart = new AssemblyPart();
            _sLDLL = assemblyPart.Load(e.Result);
            //Add the current assembly to the list of assemblies
            _assemblyList.Add(_sLDLL.FullName);
        }
#else
        /// <summary>
        /// Loads an assembly from a dll
        /// </summary>
        /// <param name="FileName">Path and name of the dll</param>
        public static void LoadFromDLL(string FileName)
        {
            byte[] bytes = File.ReadAllBytes(FileName);
            _sLDLL = Assembly.Load(bytes);
            //return _sLDLL.CreateInstance(SLProjectName);
            _assemblyList.Add(_sLDLL.FullName);
        }
#endif
        public static bool IsDLLLoaded(string AssemblyName)
        {
            foreach (string Item in _assemblyList)
            {
                if (Item.StartsWith(AssemblyName, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}