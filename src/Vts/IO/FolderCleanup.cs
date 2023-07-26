using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Vts.Test")]
[assembly: InternalsVisibleTo("Vts.MonteCarlo.CommandLineApplication.Test")]
[assembly: InternalsVisibleTo("Vts.MonteCarlo.PostProcessor.Test")]

namespace Vts.IO
{
    /// <summary>
    /// Static class with file cleanup operations
    /// </summary>
    internal static class FolderCleanup
    {
        /// <summary>
        /// Static method to delete directories using a wildcard
        /// </summary>
        /// <param name="targetDirectory">The target directory</param>
        /// <param name="wildcard">The text for the wildcard</param>
        internal static void DeleteDirectoryContaining(string targetDirectory, string wildcard)
        {
            var searchPattern = $"*{wildcard}*";
            var directoriesToDelete = Directory.EnumerateDirectories(targetDirectory, searchPattern);
            foreach (var directory in directoriesToDelete)
            {

                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        /// <summary>
        /// Static method to delete files using a wildcard
        /// </summary>
        /// <param name="targetDirectory">The target directory</param>
        /// <param name="wildcard">The text for the wildcard</param>
        internal static void DeleteFileContaining(string targetDirectory, string wildcard)
        {
            var searchPattern = $"*{wildcard}*";
            var filesToDelete = Directory.EnumerateFiles(targetDirectory, searchPattern);
            foreach (var file in filesToDelete)
            {

                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
