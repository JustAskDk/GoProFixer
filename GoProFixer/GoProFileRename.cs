using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoProFixer
{
    public class GoProFileRename
    {
        private Regex _regex = new Regex(@"(?<path>.*)GH(?<index>\d{2})(?<fileNumber>\d{4}).MP4");
        private IDictionary<string, bool> _preCalculated = new Dictionary<string, bool>();

        public bool ShouldBeRenamed(string filename)
        {
            if (!_preCalculated.ContainsKey(filename))
                _preCalculated.Add(filename, _regex.IsMatch(filename));

            return _preCalculated[filename];
        }

        public FileRenameInfo GetFileRenameInfo(string filename)
        {
            if (!ShouldBeRenamed(filename)) new FileRenameInfo(filename, filename);

            var result = _regex.Matches(filename);

            var matchCollection = _regex.Matches(filename);
            var match = matchCollection.Single();
            var newFilename = string.Format($"{match.Groups["path"]}GH{match.Groups["fileNumber"]}_{match.Groups["index"]}.MP4");

            var fileRenameInfo = new FileRenameInfo(filename, newFilename);

            return fileRenameInfo;
        }

        public IEnumerable<FileRenameInfo> GetAllFileRenameInfo(string path, SearchOption recursiveSearchOption)
        {
            var files = Directory.EnumerateFiles(path, "*.*", recursiveSearchOption);
            var fileRenameInfos = files.Where(ShouldBeRenamed).Select(GetFileRenameInfo).ToArray();
            return fileRenameInfos;
        }
    }
}