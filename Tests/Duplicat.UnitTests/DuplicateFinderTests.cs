using FsCheck;
using FsCheck.Xunit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Duplicat.UnitTests
{
    public class DuplicateFinderTests
    {
        [Property]
        public Property Returned_groups_are_not_empty(Dictionary<string, byte[]> files) =>
            new DuplicateFinder(CreateFileOpener(files))
                .Find(files.Select(f => (f.Key, f.Value.Length)))
                .All(group => group.Any())
                .ToProperty();

        [Property]
        public Property Returned_groups_where_all_elements_have_equal_content(Dictionary<string, byte[]> files) =>
            new DuplicateFinder(CreateFileOpener(files))
                .Find(files.Select(f => (f.Key, f.Value.Length)))
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

        #endregion
    }
}
