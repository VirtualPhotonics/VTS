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
            Assembly assembly = null;

            if (File.Exists(projectName + ".dll"))
            {
                assembly = Assembly.LoadFrom(projectName + ".dll");
            }
            else if(File.Exists(projectName + ".exe"))
            {
                assembly = Assembly.LoadFrom(projectName + ".exe");
            }
            else
            {
                throw new FileNotFoundException("Requested resource filestream was not found.");
            }

            var newFileName = fileName.Replace("/", ".");
            var stream = assembly.GetManifestResourceStream(projectName + "." + newFileName);
            return stream;
#endif
        }
 
        public static Stream GetFileStream(string filename, FileMode fileMode)
        {
#if SILVERLIGHT

//#if SILVERLIGHT_SAVE_TO_LOCAL_FILESTREAM_WITH_SAVEFILEDIALOG // experimental (not currently working)
//            Stream stream = null;
//            Button b = new Button { Content = "Yes" };
//            var window = new FloatableWindow
//                             {
//                                 Content = b,
//                                 Title = "Save " + filename + "?",
//                             };
//            b.Click += delegate
//                           {
//                               stream = GetFileStreamForLocalStorage(filename, fileMode);
//                               window.Close();
//                           };
            
//            window.ShowDialog();

//            return stream;
//#else
            // save to IsolatedStorage with no user interaction
            var userstore = IsolatedStorageFile.GetUserStoreForApplication();
            var locations = userstore.GetDirectoryNames();
            return new IsolatedStorageFileStream(filename, fileMode,
                IsolatedStorageFile.GetUserStoreForApplication());
//#endif

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

//#if SILVERLIGHT // experimental
//        public static Stream GetFileStreamForLocalStorage(string filename, FileMode fileMode)
//        {
//            SaveFileDialog sfd = new SaveFileDialog()
//            {
//                //DefaultExt = "txt",
//                DefaultExt = "xml",
//                //Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
//                Filter = "All files (*.*)|*.*",
//                FilterIndex = 1,
//            };

//            if (sfd.ShowDialog() == true)
//            {
//                return sfd.OpenFile();
//            }
//            else
//            {
//                return null;
//            }
//        }
//#endif
    }
}
