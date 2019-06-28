using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Duplicat
{
    public class DuplicateFinder
    {
        private static readonly HashAlgorithm _hashAlgorithm = MD5.Create();
        private readonly Func<string, Stream> _openFile;

        public DuplicateFinder(Func<string, Stream> openFile) =>
            _openFile = openFile;

        // what about zero length files?

        public IEnumerable<IEnumerable<string>> Find(IEnumerable<(string Path, int Size)> files) =>
            from file in files
            group file by file.Size into sizeDuplicates
            where sizeDuplicates.Count() > 1
            from contentDuplicates in GetContentDuplicates(sizeDuplicates.Select(f => f.Path))
            select contentDuplicates;


        private IEnumerable<IEnumerable<string>> GetContentDuplicates(IEnumerable<string> files)
        {
            if (files.Any() == false) return Enumerable.Empty<IEnumerable<string>>();

            // Split into those that match against the first element,
            // and those that don't.
            var matchesFirst = files // TODO: Cache first, don't compare against self
                //.GroupBy(f => StreamComparison(files.First(), f));
                .ToLookup(f => StreamComparison(files.First(), f));

            //return GetContentDuplicates(matchesFirst.Single(f => f.Key == false)).Prepend(matchesFirst.Single(f => f.Key == true));
            return GetContentDuplicates(matchesFirst[false]).Prepend(matchesFirst[true]);
        }


        private bool StreamComparison(string filePath1, string filePath2)
        {
            using (var stream1 = _openFile(filePath1))
            using (var stream2 = _openFile(filePath2))
            {
                var hash1 = _hashAlgorithm.ComputeHash(stream1);
                var hash2 = _hashAlgorithm.ComputeHash(stream2);
                return hash1.SequenceEqual(hash2);
            }
        }
    }
}
