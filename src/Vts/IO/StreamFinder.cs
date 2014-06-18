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
//                {"Vts.Database", "Vts.Database.dll"}
            };
        }

        /// <summary>
        /// Returns a stream from resources (standard and embedded for Silverlight and desktop, respectively),
        /// given a file name and an assembly (project) name
        /// </summary>
        /// <param name="fileName">Name of the file in resources</param>
        /// <param name="projectName">Project name where the resources are located</param>
        /// <returns>Stream of data</returns>
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
            string fullPath = Path.Combine(currentAssemblyDirectoryName, projectName);

            Assembly assembly = null;

            if (File.Exists(fullPath + ".dll"))
            {
                assembly = Assembly.LoadFrom(fullPath + ".dll");
            }
            else if (File.Exists(fullPath + ".exe"))
            {
                assembly = Assembly.LoadFrom(fullPath + ".exe");
            }
            else if (FileIO.FileExists(projectName + ".dll"))
            {
                assembly = Assembly.LoadFrom(projectName + ".dll");
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
        /// <param name="filename">Name of the file</param>
        /// <param name="fileMode">The FileMode to use when accessing the file</param>
        /// <returns>Stream of data</returns>
        public static Stream GetFileStream(string filename, FileMode fileMode)
        {
#if SILVERLIGHT
            // save to IsolatedStorage with no user interaction
            var userstore = IsolatedStorageFile.GetUserStoreForApplication();
            var locations = userstore.GetDirectoryNames();
            return new IsolatedStorageFileStream(filename, fileMode, IsolatedStorageFile.GetUserStoreForApplication());
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
        /// <returns>Stream of data</returns>
        public static Stream GetLocalFilestreamFromSaveFileDialog(string defaultExtension)
        {
            var dialog = new SaveFileDialog();
            dialog.DefaultExt = defaultExtension;
            //dialog.Filter = dialog.DefaultExt + " File|*" + dialog.DefaultExt + "|All Files|*.*";
            dialog.Filter = defaultExtension + " files (*." + defaultExtension + ")|*." + defaultExtension + "|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                return dialog.OpenFile();
            }
            return null;
        }

        /// <summary>
        /// Displays the Load File dialog box to select or create a file, returns a file stream to write to that file.
        /// </summary>
        /// <param name="defaultExtension">A string representing the default file name extension of the file to be saved.</param>
        /// <returns>Stream of data</returns>
        public static Stream GetLocalFilestreamFromOpenFileDialog(string defaultExtension)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = defaultExtension + " files (*." + defaultExtension + ")|*." + defaultExtension + "|Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            //dialog.Filter = defaultExtension + " File|*" + defaultExtension + "|All Files|*.*"; //"Text Files (.txt)|*.txt|All Files (*.*)|*.*";

            dialog.FilterIndex = 2;
            dialog.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = dialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                System.IO.Stream fileStream = dialog.File.OpenRead();

                return fileStream;
            }
            return null;
        }
#endif
    }
}
