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

        public IEnumerable<IEnumerable<string>> Find(IEnumerable<(string Path, int Size)> files) =>
            files.Select(_ => files.Select(f => f.Path));
            //files.Select(f => new[] { f.Path });
            //throw new NotImplementedException();
    }
}
