using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace GoProFixer
{
    public class FileRenameInfo
    {
        public FileRenameInfo(string originalName, string newName)
        {
            if (string.IsNullOrEmpty(originalName))
                throw new ArgumentException($"'{nameof(originalName)}' cannot be null or empty.", nameof(originalName));

            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException($"'{nameof(newName)}' cannot be null or empty.", nameof(newName));

            OriginalName = originalName;
            NewName = newName;
        }

        public string OriginalName { get; private set; }

        public string NewName { get; private set; }

        public void PerformRename()
        {
            File.Move(OriginalName, NewName);
        }
    }

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

        class Program
        {
            static void Main(string[] args)
            {
                ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);

                if (parserResult.Errors.Any())
                {
                    throw new InvalidOperationException();
                }

                var path = parserResult.Value.Path;

                var strings = Directory.GetFiles(path);

                var regex = new Regex(@"(?<path>.*)GH(?<index>\d{2})(?<fileNumber>\d{4}).MP4");

                foreach (var originalFilename in strings)
                {
                    var matchCollection = regex.Matches(originalFilename);

                    if (matchCollection.Count == 1)
                    {
                        var single = matchCollection.Single();
                        var newFilename = string.Format("{2}GH{0}_{1}.MP4",
                            single.Groups["fileNumber"], single.Groups["index"], single.Groups["path"]);
                        Console.WriteLine("Old: " + originalFilename + " New: " + newFilename);
                        File.Move(originalFilename, newFilename);
                    }
                }
            }
        }
    }
}