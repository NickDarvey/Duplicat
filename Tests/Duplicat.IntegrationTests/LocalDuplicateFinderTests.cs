﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Duplicat.IntegrationTests
{
    public class LocalDuplicateFinderTests
    {
        [Fact]
        public void Should_group_duplicates_by_content()
        {
            var contents = new[]
            {
                new byte[] { },
                new byte[] { 0x01 },
                new byte[] { 0x01, 0x02 },
                new byte[] { 0x01, 0x02, 0x03 },
            };

            using var samples = TemporarySampleFiles.Create(new[]
            {
                contents[0],
                contents[0],
                contents[0],

                contents[1],
                contents[1],

                contents[2],

                contents[3],
            });

            var expected = new[]
            {
                new []
                {
                    samples.ElementAt(0),
                    samples.ElementAt(1),
                    samples.ElementAt(2),
                },
                new []
                {
                    samples.ElementAt(3),
                    samples.ElementAt(4),
                },
            };


            var results = LocalDuplicateFinder.Find(path: samples.DirectoryPath, pattern: samples.FilePattern, recurse: true);


            Assert.Equal(expected, results);
        }

        [Theory]
        [InlineData("\023")]
        [InlineData("\0")]
        [InlineData("?")]
        public void Should_throw_for_invalid_directory_paths(string directoryPath)
        {
            Assert.Throws<ArgumentException>(() => LocalDuplicateFinder.Find(directoryPath, "*.*", recurse: false));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData(null)]
        public void Should_throw_for_empty_directory_paths(string directoryPath)
        {
            Assert.Throws<ArgumentNullException>(() => LocalDuplicateFinder.Find(directoryPath, "*.*", recurse: false));
        }


        #region Utilities

        private class TemporarySampleFiles : IEnumerable<string>, IDisposable
        {
            private readonly IEnumerable<string> _filePaths;

            private TemporarySampleFiles(
                IEnumerable<string> filePaths,
                string directoryPath)
            {
                _filePaths = filePaths;
                DirectoryPath = directoryPath;
            }

            public string DirectoryPath { get; }
            public string FilePattern { get; } = "*.*";

            public static TemporarySampleFiles Create(byte[][] contents)
            {
                var directoryPath = Path.Join(Path.GetTempPath(), nameof(LocalDuplicateFinderTests));
                Directory.CreateDirectory(directoryPath);

                var filePaths = new List<string>();
                for (int i = 0; i < contents.Length; i++)
                {
                    var filePath = Path.Join(directoryPath, $"{i}.tmp");
                    File.WriteAllBytes(filePath, contents[i]);
                    filePaths.Add(filePath);
                }

                return new TemporarySampleFiles(filePaths, directoryPath);
            }

            public IEnumerator<string> GetEnumerator() => _filePaths.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _filePaths.GetEnumerator();

            public void Dispose() => Directory.Delete(DirectoryPath, recursive: true);
        }

        #endregion
    }
}
