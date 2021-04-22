using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoProFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            var strings = Directory.GetFiles("c:\\temp\\test\\");

            
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
                    File.Move(originalFilename,newFilename);
                }
            }
        }
    }
}