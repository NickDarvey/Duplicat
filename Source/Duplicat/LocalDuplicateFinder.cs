using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

namespace Duplicat
{
    /// <summary>
    /// A local file system implementation of a duplicate finder.
    /// </summary>
    public static class LocalDuplicateFinder
    {
        private static readonly DuplicateFinder _finder = new DuplicateFinder(File.OpenRead);

        /// <summary>
        /// Tries to find duplicates in a directory by its path.
        /// </summary>
        /// <param name="directoryPath">The path for the directory in which to search.</param>
        /// <param name="recurse">True, will search subdirectories. False, will search top-level directory only.</param>
        /// <param name="results">Groups of duplicate files, if true was returned. Otherwise default.</param>
        /// <param name="errors">The errors, if false was returned. Otherwise default.</param>
        /// <returns>True, if search was successful. False, if it failed.</returns>
        public static bool TryFind(string directoryPath, bool recurse, out IEnumerable<IEnumerable<string>> results, out IEnumerable<string> errors)
        {
            if (IsValidPath(directoryPath) == false)
            {
                results = default;
                errors = new[] { $"Directory '${directoryPath}' is not a valid path." };
                return false;
            }

            if (Directory.Exists(directoryPath) == false)
            {
                results = default;
                errors = new[] { $"Directory '${directoryPath}' does not exist." };
                return false;
            }

            var files = new DirectoryInfo(directoryPath)
                .EnumerateFiles("*", new EnumerationOptions { RecurseSubdirectories = recurse })
                .Select(f => (f.FullName, f.Length));

            results = _finder.Find(files);
            errors = default;
            return true;
        }

        private static bool IsValidPath(string directoryPath)
        {
            try
            {
                _ = Path.GetFullPath(directoryPath); // Throws on invalid paths
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is NotSupportedException ||
                ex is PathTooLongException ||
                ex is SecurityException)
            {
                return false;
            }
        }
    }
}
