using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Duplicat
{
    /// <summary>
    /// A local file system implementation of a duplicate finder.
    /// </summary>
    public static class LocalDuplicateFinder
    {
        private static readonly DuplicateFinder _finder = new DuplicateFinder(File.OpenRead);

        public static IEnumerable<IEnumerable<string>> Find(string path, string pattern, bool recurse) =>
            string.IsNullOrWhiteSpace(path) ? throw new ArgumentNullException(nameof(path))
            : Directory.Exists(path) == false ? throw new ArgumentException($"Directory '{path}' does not exist", nameof(path))
            : _finder.Find(new DirectoryInfo(path)
                .EnumerateFiles(pattern, new EnumerationOptions { RecurseSubdirectories = recurse })
                .Select(f => (f.FullName, f.Length)));
    }
}
