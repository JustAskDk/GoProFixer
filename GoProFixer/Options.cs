using CommandLine;

namespace GoProFixer
{
    public class Options
    {
        [Option('p', "path", Required = true, HelpText = "Path to files to rename")]
        public string Path { get; set; }

        [Option('d', "dryRun", Required = true, HelpText = "Dry run rename - Only shows planned changes")]
        public bool DryRun { get; set; }
    }
}