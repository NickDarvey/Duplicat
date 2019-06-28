using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Duplicat
{
    public class DuplicateFinder
    {
        private readonly Func<string, Stream> _openFile;

        public DuplicateFinder(Func<string, Stream> openFile) =>
            _openFile = openFile;

        public IEnumerable<IEnumerable<string>> Find(IEnumerable<(string Path, long Size)> files) =>
            from file in files
            group file by file.Size into sizeDuplicates // TODO: https://stackoverflow.com/a/3433635
            where sizeDuplicates.Count() > 1
            from contentDuplicates in GetContentDuplicates(sizeDuplicates.Select(f => f.Path))
            where contentDuplicates.Count() > 1
            select contentDuplicates;


        private IEnumerable<IEnumerable<string>> GetContentDuplicates(IEnumerable<string> files)
        {
            if (files.Any() == false) return Enumerable.Empty<IEnumerable<string>>();

            // Split into those that match against the first element, and those that don't.
            var matchesFirst = files // TODO: Cache first, don't compare against self
                .ToLookup(f => StreamComparison(files.First(), f));

            return GetContentDuplicates(matchesFirst[false]).Prepend(matchesFirst[true]); // TODO: Find a non-recursive solution
        }

        private bool StreamComparison(string filePath1, string filePath2)
        {
            using var stream1 = _openFile(filePath1);
            using var stream2 = _openFile(filePath2);
            return stream1.AsEnumerable().SequenceEqual(stream2.AsEnumerable());
        }    
    }
}
