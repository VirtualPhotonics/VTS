using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vts.MonteCarlo.CommandLineApplication.Test.Helpers
{
    internal static class Files
    {
        public static void DeleteDirectoryContaining(string targetDirectory, string wildcard)
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

        public static void DeleteFileContaining(string targetDirectory, string wildcard)
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
