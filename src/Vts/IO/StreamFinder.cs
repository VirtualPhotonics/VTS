using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
#if SILVERLIGHT
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
#else

#endif

namespace Vts.IO
{
    /// <summary>
    /// Implements static functions for returning streams from various locations (resources, file system, etc)
    /// </summary>
    public static class StreamFinder
    {
        // todo: this is a temp location for this resource list, should be moved to .resx file
        private static IDictionary<string, string> _lazyLoadLibraries;

        static StreamFinder()
        {
            // dictionary of libraries to be lazy loaded, keyed by the project name
            _lazyLoadLibraries = new Dictionary<string, string>
            {
                {"Vts.Database", "Vts.Database.dll"}
            };
        }

        /// <summary>
        /// Returns a stream from resources (standard and embedded for Silverlight and desktop, respectively),
        /// given a file name and an assembly (project) name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static Stream GetFileStreamFromResources(string fileName, string projectName)
        {
            if (_lazyLoadLibraries.ContainsKey(projectName))
            {
                LibraryIO.EnsureDllIsLoaded(_lazyLoadLibraries[projectName]);
            }

#if SILVERLIGHT
            var stream = Application.GetResourceStream(new Uri(projectName + ";component/" + fileName, UriKind.Relative)).Stream;
            return stream;
#else
            string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = currentAssemblyDirectoryName + "\\" + projectName;

            Assembly assembly = null;

            if (File.Exists(fullPath + ".dll"))
            {
                assembly = Assembly.LoadFrom(fullPath + ".dll");
            }
            else if (File.Exists(fullPath + ".exe"))
            {
                assembly = Assembly.LoadFrom(fullPath + ".exe");
            }
            else
            {
                throw new FileNotFoundException("Can't locate specified assembly.");
            }

            var newFileName = fileName.Replace("/", ".").Replace(@"\", ".");
            var stream = assembly.GetManifestResourceStream(projectName + "." + newFileName);
            return stream;
#endif
        }

        /// <summary>
        /// Returns a stream from the local file system. In the case of Silverlight, this stream is from isolated storage.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="fileMode"></param>
        /// <returns></returns>
        public static Stream GetFileStream(string filename, FileMode fileMode)
        {
#if SILVERLIGHT
            // save to IsolatedStorage with no user interaction
            var userstore = IsolatedStorageFile.GetUserStoreForApplication();
            var locations = userstore.GetDirectoryNames();
            return new IsolatedStorageFileStream(filename, fileMode,
                IsolatedStorageFile.GetUserStoreForApplication());
#else
            FileStream fs = null;

            try
            {
                fs = File.Open(filename, fileMode);
            }
            catch
            {
                // problem locating file
            }

            if (fs != null)
            {
                return fs;
            }

            try
            {
                string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string fullPath = currentAssemblyDirectoryName + "\\" + filename;
                fs = File.Open(fullPath, fileMode);
            }
            catch
            {
                // problem locating file
            }

            if (fs != null)
            {
                return fs;
            }

            return null;
#endif
        }

#if SILVERLIGHT
        /// <summary>
        /// Displays the Save File dialog box to select or create a file, returns a file stream to write to that file.
        /// </summary>
        /// <param name="defaultExtension">A string representing the default file name extension of the file to be saved.</param>
        /// <returns>System.IO.Stream</returns>
        public static Stream GetLocalFilestreamFromFileDialog(string defaultExtension)
        {
            var dialog = new SaveFileDialog();
            dialog.DefaultExt = defaultExtension;
            dialog.Filter = dialog.DefaultExt + " File|*" + dialog.DefaultExt + "|All Files|*.*";

            if (dialog.ShowDialog() == true)
            {
                return dialog.OpenFile();
            }
            return null;
        }
#endif
    }
}
