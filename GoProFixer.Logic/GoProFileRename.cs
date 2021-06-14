using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoProFixer.Logic
{
    public class GoProFileRename
    {
        private readonly Regex _regex1 = new Regex(@"(?<path>.*)(?<preFix>(G[(L)|(H)|(X)|(P)]))(?<index>\d{2})(?<fileNumber>\d{4}).(?<fileType>(MP4)|(THM)|(LRV))$");
        private readonly Regex _regex2 = new Regex(@"(?<path>.*)(?<preFix>GOPR)(?<fileNumber>\d{4}).(?<fileType>(MP4)|(THM)|(LRV))$");
        private readonly IDictionary<string, bool> _preCalculated = new Dictionary<string, bool>();

        public bool ShouldBeRenamed(string filename)
        {
            if (!_preCalculated.ContainsKey(filename))
                _preCalculated.Add(filename, _regex1.IsMatch(filename) || _regex2.IsMatch(filename));

            return _preCalculated[filename];
        }

        public FileRenameInfo GetFileRenameInfo(string filename)
        {
            if (!ShouldBeRenamed(filename)) new FileRenameInfo(filename, filename);

            if (_regex1.IsMatch(filename))
            {
                var result = _regex1.Matches(filename);

                var matchCollection = _regex1.Matches(filename);
                var match = matchCollection.Single();
                var newFilename = string.Format($"{match.Groups["path"]}{match.Groups["preFix"]}{match.Groups["fileNumber"]}_{match.Groups["index"]}.{match.Groups["fileType"]}");

                var fileRenameInfo = new FileRenameInfo(filename, newFilename);

                return fileRenameInfo;
            }
            else if (_regex2.IsMatch(filename))
            {
                var result = _regex2.Matches(filename);

                var matchCollection = _regex2.Matches(filename);
                var match = matchCollection.Single();
                var newFilename = string.Format($"{match.Groups["path"]}GP{match.Groups["fileNumber"]}_00.{match.Groups["fileType"]}");

                var fileRenameInfo = new FileRenameInfo(filename, newFilename);

                return fileRenameInfo;
            }

            throw new InvalidOperationException("How did you get here?!?");
        }

        public IEnumerable<FileRenameInfo> GetAllFileRenameInfo(string path, SearchOption recursiveSearchOption)
        {
            var files = Directory.EnumerateFiles(path, "*.*", recursiveSearchOption);
            var fileRenameInfos = files.Where(ShouldBeRenamed).Select(GetFileRenameInfo).ToArray();
            return fileRenameInfos;
        }
    }
}