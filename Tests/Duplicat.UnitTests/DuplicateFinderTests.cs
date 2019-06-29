using FsCheck;
using FsCheck.Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Duplicat.UnitTests
{
    public class DuplicateFinderTests
    {
        [Property]
        public Property Returned_groups_contain_more_than_one_element(Dictionary<string, byte[]> files) =>
            new DuplicateFinder(CreateFileOpener(files))
                .Find(CreateFileSummaries(files))
                .All(group => group.Count() > 1)
                .ToProperty();

        [Property]
        public Property Returned_groups_where_all_elements_have_equal_content(Dictionary<string, byte[]> files) =>
            new DuplicateFinder(CreateFileOpener(files))
                .Find(CreateFileSummaries(files))
                .All(group => group.Select(path => files[path]).Distinct(ByteArrayEqualityComparer.Instance).Count() <= 1)
                .ToProperty();

        #region Utilities

        private class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
        {
            public static ByteArrayEqualityComparer Instance { get; } = new ByteArrayEqualityComparer();

            public bool Equals(byte[] x, byte[] y) => x.SequenceEqual(y);

            public int GetHashCode(byte[] obj)
            {
                // HashCode based on contents
                // https://stackoverflow.com/a/3329582
                var result = 13 * obj.Length;
                for (var i = 0; i < obj.Length; i++)
                    result = (17 * result) + obj[i];
                return result;
            }
        }

        private static Func<string, Stream> CreateFileOpener(IReadOnlyDictionary<string, byte[]> files) =>
            path => new MemoryStream(files[path]);

        private static IEnumerable<(string Path, long Size)> CreateFileSummaries(Dictionary<string, byte[]> files) =>
            files.Select(f => (f.Key, (long)f.Value.Length));

        #endregion
    }
}
