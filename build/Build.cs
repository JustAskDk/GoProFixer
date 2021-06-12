using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.CompressionTasks;
using Nuke.Common.CI.AzurePipelines;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[AzurePipelines(AzurePipelinesImage.WindowsLatest, AutoGenerate = false, InvokedTargets = new[] { "Compile" })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
     
    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath ArtifactsDirectory => RootDirectory / "~" / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Console.WriteLine("Working dir:" + System.IO.Directory.GetCurrentDirectory());
            System.IO.Directory.CreateDirectory(@".\~\.nuget\packages");

            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Produces(RootDirectory / "GoProFixer.Logic" / "bin" / "Debug" / "net5.0" / "*.exe")
        .Executes(() =>
        {
        DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .EnableNoRestore());
            var files = RootDirectory / "GoProFixer.Logic";
            var test = System.IO.Directory.GetFiles(files);
            Console.WriteLine("Files: " + string.Join(", ", test));

            Console.WriteLine("Compressing to: " + ArtifactsDirectory);
            CompressZip(files, ArtifactsDirectory / "TestArchive.zip");
        });

}
