using System;
using System.IO;

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
}