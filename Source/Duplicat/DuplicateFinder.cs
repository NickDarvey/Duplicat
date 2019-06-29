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
            group file by file.Size into sizeDuplicates
            where sizeDuplicates.Count() > 1
            from contentDuplicates in GetContentDuplicates(sizeDuplicates.Select(f => f.Path))
            where contentDuplicates.Count() > 1
            select contentDuplicates;

        private IEnumerable<IEnumerable<string>> GetContentDuplicates(IEnumerable<string> files)
        {
            var accumulated = Enumerable.Empty<IEnumerable<string>>();
            while (files.Any() != false)
            {
                // Split into those that match against the first element, and those that don't.
                // TODO: Cache streams; don't first stream against itself.
                var matchesFirst = files.ToLookup(f => StreamComparison(files.First(), f));

                files = matchesFirst[false];
                accumulated = accumulated.Prepend(matchesFirst[true]);
            }
            return accumulated;
        }

        private bool StreamComparison(string filePath1, string filePath2)
        {
            using (var stream1 = _openFile(filePath1))
            using (var stream2 = _openFile(filePath2))
            {
                return stream1.AsEnumerable().SequenceEqual(stream2.AsEnumerable());
            }
        }
    }
}
