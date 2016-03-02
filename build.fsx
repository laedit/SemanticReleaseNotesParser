#r "./tools/FAKE/tools/FakeLib.dll"
#load "NVikaHelper.fsx"
#load "SemanticReleaseNotesParserHelper.fsx"

open Fake
open Fake.Testing.XUnit2
open Fake.NuGet.Install
open OpenCoverHelper
open NVikaHelper
open SemanticReleaseNotesParserHelper

// Properties
let buildDir = "./build/"
let testDir  = "./test/"
let artifactsDir = "./artifacts/"

// version info
let version = if isLocalBuild then "0.0.1" else if buildServer = AppVeyor then environVar "GitVersion_NuGetVersionV2" else buildVersion
let tag = if buildServer = AppVeyor then AppVeyor.AppVeyorEnvironment.RepoTagName else "v0.0.1"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; artifactsDir]
)

Target "RestorePackages" (fun _ ->
    "./src/SemanticReleaseNotesParser.sln"
    |> RestoreMSSolutionPackages (fun p -> { p with OutputPath = "src/packages" })
)

Target "BuildApp" (fun _ ->
    !! "src/SemanticReleaseNotesParser/SemanticReleaseNotesParser.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "InspectCodeAnalysis" (fun _ ->
    "resharper-clt.portable" |> Choco.Install id
    
    if directExec(fun info ->
        info.FileName <- "inspectcode"
        info.Arguments <- "/o=\"" + artifactsDir + "inspectcodereport.xml\" \"src\SemanticReleaseNotesParser.sln\"" )
    then
        "nvika" |> Choco.Install id
        artifactsDir + "inspectcodereport.xml" |> NVika.ParseReport (fun p -> { p with Debug = true; IncludeSource = true })
            
    else failwith "Execution of inspectcode have failed, NVika can't be executed."
)

Target "ILRepack" (fun _ ->
    "ilrepack" |> NugetInstall (fun p -> 
    { p with 
        OutputDirectory = "tools";
        ExcludeVersion = true
    })
    
    if not (directExec(fun info ->
        info.FileName <- "./tools/ilrepack/tools/ilrepack" 
        info.Arguments <- @"/internalize /parallel /wildcards /lib:""C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable\v4.0"" /out:" + artifactsDir + "SemanticReleaseNotesParser.exe " + buildDir + "SemanticReleaseNotesParser.exe " + buildDir + "*.dll" ))
    then
        failwith "Execution of ilrepack have failed."
)

Target "BuildReleaseNotes" (fun _ ->
     SemanticReleaseNotesParser.Convert (fun p -> { p with 
                                                        GroupBy = SemanticReleaseNotesParser.GroupByType.Categories
                                                        Debug = true
                                                        OutputPath = artifactsDir @@ "ReleaseNotes.html"
                                                        PluralizeCategoriesTitle = true
                                                        IncludeStyle = SemanticReleaseNotesParser.IncludeStyleType.Yes
                                                        ToolPath = artifactsDir @@ "SemanticReleaseNotesParser.exe" 
        } )

     SemanticReleaseNotesParser.Convert (fun p -> { p with 
                                                        GroupBy = SemanticReleaseNotesParser.GroupByType.Categories
                                                        OutputType = SemanticReleaseNotesParser.OutputType.Environment
                                                        OutputFormat = SemanticReleaseNotesParser.OutputFormat.Markdown
                                                        Debug = true
                                                        PluralizeCategoriesTitle = true
                                                        ToolPath = artifactsDir @@ "SemanticReleaseNotesParser.exe" 
        } )
)

Target "BuildTest" (fun _ ->
    !! "src/*.Tests/*.Tests.csproj"
      |> MSBuildRelease testDir "Build"
      |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    "xunit.runner.console" |> NugetInstall (fun p -> 
    { p with 
        OutputDirectory = "tools";
        ExcludeVersion = true
    })
    
    if isUnix then
        !! (testDir @@ "SemanticReleaseNotesParser*.Tests.dll")
        |> xUnit2 (fun p -> { p with 
                                HtmlOutputPath = Some (artifactsDir @@ "xunit.html") 
                                ShadowCopy = false
                            })
    else

        "OpenCover" |> NugetInstall (fun p -> 
        { p with 
            OutputDirectory = "tools";
            ExcludeVersion = true
        })
    
        testDir + "SemanticReleaseNotesParser.Core.Tests.dll -noshadow" |> OpenCover (fun p -> 
        { p with
            ExePath = "./tools/OpenCover/tools/OpenCover.Console.exe"
            TestRunnerExePath = "./tools/xunit.runner.console/tools/xunit.console.exe";
            Output = artifactsDir @@ "coverage.xml";
            Register = RegisterUser;
            Filter = "+[SemanticReleaseNotesParser.Core]*";
            OptionalArguments = "-excludebyattribute:*.ExcludeFromCodeCoverage* -returntargetcode";
        })
    
        testDir + "SemanticReleaseNotesParser.Tests.dll -noshadow" |> OpenCover (fun p -> 
        { p with
            ExePath = "./tools/OpenCover/tools/OpenCover.Console.exe"
            TestRunnerExePath = "./tools/xunit.runner.console/tools/xunit.console.exe";
            Output = artifactsDir @@ "coverage.xml";
            Register = RegisterUser;
            Filter = "+[SemanticReleaseNotesParser]*";
            OptionalArguments = "-excludebyattribute:*.ExcludeFromCodeCoverage* -returntargetcode -mergeoutput";
        })
    
        if isLocalBuild then
            "ReportGenerator" |> NugetInstall (fun p -> 
            { p with 
                OutputDirectory = "tools";
                ExcludeVersion = true
            })
            [artifactsDir @@ "coverage.xml"] |> ReportGeneratorHelper.ReportGenerator (fun p -> 
            { p with 
                TargetDir = artifactsDir @@ "reports" 
                ExePath = @"tools\ReportGenerator\tools\ReportGenerator.exe"
                LogVerbosity = ReportGeneratorHelper.ReportGeneratorLogVerbosity.Error
            })
        else
            "coveralls.io" |> NugetInstall (fun p -> 
            { p with 
                OutputDirectory = "tools";
                ExcludeVersion = true
            })
            if not (directExec(fun info ->
                info.FileName <- @"tools\coveralls.io\tools\coveralls.net.exe"
                info.Arguments <- "--opencover " + artifactsDir + "coverage.xml" ))
            then
                failwith "Execution of coveralls.net have failed."
)

Target "Zip" (fun _ ->
    !! (artifactsDir + "SemanticReleaseNotesParser.exe")
    ++ (artifactsDir + "ReleaseNotes.html")
    |> Zip artifactsDir (artifactsDir + "SemanticReleaseNotesParser." + version + ".zip")
)

Target "PackFakeHelper" (fun _ ->
    "SemanticReleaseNotesParserHelper.fsx" |> FileHelper.CopyFile artifactsDir
    artifactsDir @@ "SemanticReleaseNotesParserHelper.fsx" |> FileHelper.RegexReplaceInFileWithEncoding "./tools/FAKE/tools/FakeLib.dll" "FakeLib.dll" System.Text.Encoding.UTF8
)

Target "ChocoPack" (fun _ ->
    Choco.Pack (fun p -> 
        { p with 
            PackageId = "semanticreleasenotes-parser"
            Version = version
            Title = "Semantic Release Notes Parser"
            Authors = ["laedit"]
            Owners = ["laedit"]
            ProjectUrl = "https://github.com/laedit/SemanticReleaseNotesParser"
            IconUrl = "https://cdn.rawgit.com/laedit/SemanticReleaseNotesParser/master/icon.png"
            LicenseUrl = "https://github.com/laedit/SemanticReleaseNotesParser/blob/master/LICENSE"
            BugTrackerUrl = "https://github.com/laedit/https://github.com/laedit/SemanticReleaseNotesParser/blob/master/LICENSE/issues"
            Description = "Parser for Semantic Release Notes (http://www.semanticreleasenotes.org/)"
            Tags = ["Semantic"; "Release"; "Notes"; "Parser"; "hmtl"; "markdown"; "liquid"]
            ReleaseNotes = "https://github.com/laedit/SemanticReleaseNotesParser/releases"
            PackageDownloadUrl = "https://github.com/laedit/SemanticReleaseNotesParser/releases/download/" + tag + "/SemanticReleaseNotesParser." + version + ".zip"
            OutputDir = artifactsDir
        })
)

Target "NugetPack" (fun _ ->
    NuGet (fun p ->
    { p with
        Version = version
        OutputPath = artifactsDir
        WorkingDir = buildDir
    }) "src/SemanticReleaseNotesParser.Core/SemanticReleaseNotesParser.Core.csproj"
)

Target "All" DoNothing

// Dependencies
"Clean" ==> "ChocoPack"

"Clean"
  ==> "RestorePackages"
  ==> "BuildApp"
  =?> ("InspectCodeAnalysis", Choco.IsAvailable)
  =?> ("ILRepack", Choco.IsAvailable)
  ==> "BuildReleaseNotes"
  ==> "BuildTest"
  ==> "Test"
  ==> "Zip"
  ==> "PackFakeHelper"
  =?> ("ChocoPack", Choco.IsAvailable)
  ==> "NugetPack"
  ==> "All"

// start build
RunTargetOrDefault "All"
