using System;
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
    public static class StreamFinder
    {
        public static Stream GetFileStreamFromResources(string fileName, string projectName)
        {
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

            var newFileName = fileName.Replace("/", ".");
            var stream = assembly.GetManifestResourceStream(projectName + "." + newFileName);
            return stream;
#endif
        }
 
        public static Stream GetFileStream(string filename, FileMode fileMode)
        {
#if SILVERLIGHT
            // save to IsolatedStorage with no user interaction
            var userstore = IsolatedStorageFile.GetUserStoreForApplication();
            var locations = userstore.GetDirectoryNames();
            return new IsolatedStorageFileStream(filename, fileMode,
                IsolatedStorageFile.GetUserStoreForApplication());
#else
            return File.Open(Path.GetFullPath(filename), fileMode);
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
