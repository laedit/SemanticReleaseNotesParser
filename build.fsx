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
// done
Target "RestorePackages" (fun _ ->
    "./src/SemanticReleaseNotesParser.sln"
    |> RestoreMSSolutionPackages (fun p -> { p with OutputPath = "src/packages" })
)
// done
Target "PrepareSourceLink" (fun _ ->
    "sourceLink" |> Choco.Install id

    if not (directExec(fun info ->
        info.FileName <- "SourceLink" 
        info.Arguments <- "linefeed -pr ./src/SemanticReleaseNotesParser.Core/SemanticReleaseNotesParser.Core.csproj"))
    then
        failwith "Execution of SourceLink have failed."
)
// done
Target "BuildApp" (fun _ ->
    !! "src/SemanticReleaseNotesParser.Core/SemanticReleaseNotesParser.Core.csproj"
      |> MSBuildRelease null "ReBuild"
      |> Log "AppBuild-Output: "

    !! "src/SemanticReleaseNotesParser/SemanticReleaseNotesParser.csproj"
      |> MSBuildRelease buildDir "ReBuild"
      |> Log "AppBuild-Output: "
)
// to do after
Target "InspectCodeAnalysis" (fun _ ->
    "nvika" |> Choco.Install id
    [buildDir + "static-analysis.Core.sarif.json"; buildDir + "static-analysis.Program.sarif.json"] |> NVika.ParseReports (fun p -> { p with Debug = true; IncludeSource = true })
)
// done
Target "ILRepack" (fun _ ->
    "ilrepack" |> NugetInstall (fun p -> 
    { p with 
        OutputDirectory = "tools";
        ExcludeVersion = true
    })
    
    if not (directExec(fun info ->
        info.FileName <- "./tools/ilrepack/tools/ilrepack" 
        info.Arguments <- @"/internalize /parallel /wildcards /lib:""C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable\v4.0"" /out:" + artifactsDir + "SemanticReleaseNotesParser.exe " + (buildDir @@ "SemanticReleaseNotesParser.exe ") + (buildDir @@ "*.dll") ))
    then
        failwith "Execution of ilrepack have failed."
)
// done
Target "SourceLink" (fun _ ->
    if not (directExec(fun info ->
        info.FileName <- "SourceLink" 
        info.Arguments <- "index -pr ./src/SemanticReleaseNotesParser.Core/SemanticReleaseNotesParser.Core.csproj -pp Configuration Release -u \"https://raw.githubusercontent.com/laedit/semanticreleasenotesparser/{0}/%var2%\""))
    then
        failwith "Execution of SourceLink have failed."
)
// done
Target "BuildReleaseNotes" (fun _ ->
     SemanticReleaseNotesParser.Convert (fun p -> { p with 
                                                        GroupBy = SemanticReleaseNotesParser.GroupByType.Categories
                                                        Debug = true
                                                        OutputPath = artifactsDir @@ "ReleaseNotes.html"
                                                        PluralizeCategoriesTitle = true
                                                        IncludeStyle = SemanticReleaseNotesParser.IncludeStyleType.Yes
                                                        ToolPath = if Choco.IsAvailable then artifactsDir @@ "SemanticReleaseNotesParser.exe" else buildDir @@ "SemanticReleaseNotesParser.exe"
        } )

     SemanticReleaseNotesParser.Convert (fun p -> { p with 
                                                        GroupBy = SemanticReleaseNotesParser.GroupByType.Categories
                                                        OutputType = SemanticReleaseNotesParser.OutputType.Environment
                                                        OutputFormat = SemanticReleaseNotesParser.OutputFormat.Markdown
                                                        Debug = true
                                                        PluralizeCategoriesTitle = true
                                                        ToolPath = if Choco.IsAvailable then artifactsDir @@ "SemanticReleaseNotesParser.exe" else buildDir @@ "SemanticReleaseNotesParser.exe"
        } )
)

// done
Target "BuildTest" (fun _ ->
    !! "src/*.Tests/*.Tests.csproj"
      |> MSBuildRelease testDir "Build"
      |> Log "AppBuild-Output: "
)
 // half done, report missing
Target "Test" (fun _ ->
    "xunit.runner.console" |> NugetInstall (fun p -> 
    { p with 
        OutputDirectory = "tools";
        ExcludeVersion = true
    })
    // done
    if isUnix then
        !! (testDir @@ "SemanticReleaseNotesParser.Core.Tests.dll")
        |> xUnit2 (fun p -> { p with 
                                HtmlOutputPath = Some (artifactsDir @@ "xunit.html") 
                                ShadowCopy = false
                            })
    else
    // done
        "OpenCover" |> NugetInstall (fun p -> 
        { p with 
            OutputDirectory = "tools";
            ExcludeVersion = true
        })
        // done
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
    // done
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
        // todo https://github.com/marketplace/actions/codecov / single report merged by reportgenerator or upload each report
            System.Environment.SetEnvironmentVariable("PATH", ("C:\\Python34;C:\\Python34\\Scripts;" + environVar "PATH"))
            if not (directExec(fun info ->
                info.FileName <- "pip"
                info.Arguments <- "install codecov" ))
            then
                failwith "Installation of codecov have failed."
            if not (directExec(fun info ->
                info.FileName <- "codecov"
                info.Arguments <- "-f " + artifactsDir + "coverage.xml -X gcov" ))
            then
                failwith "Execution of codecov have failed."
)
// done
Target "Zip" (fun _ ->
    !! (artifactsDir + "SemanticReleaseNotesParser.exe")
    ++ (artifactsDir + "ReleaseNotes.html")
    |> Zip artifactsDir (artifactsDir + "SemanticReleaseNotesParser." + version + ".zip")
)

// discontinued
Target "PackFakeHelper" (fun _ ->
    "SemanticReleaseNotesParserHelper.fsx" |> FileHelper.CopyFile artifactsDir
    artifactsDir @@ "SemanticReleaseNotesParserHelper.fsx" |> FileHelper.RegexReplaceInFileWithEncoding "./tools/FAKE/tools/FakeLib.dll" "FakeLib.dll" System.Text.Encoding.UTF8
)

// current https://github.com/crazy-max/ghaction-chocolatey/blob/master/src/main.ts
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
            Checksum = Checksum.CalculateFileHash (artifactsDir + "SemanticReleaseNotesParser." + version + ".zip")
            ChecksumType = Choco.ChocolateyChecksumType.Sha256
        })
)
// done
Target "NugetPackCore" (fun _ ->
    NuGet (fun p ->
    { p with
        Version = version
        OutputPath = artifactsDir
        WorkingDir = buildDir
        Properties = ["Configuration","Release"]
    }) "src/SemanticReleaseNotesParser.Core/SemanticReleaseNotesParser.Core.csproj"
)
// done
Target "NugetPackFake" (fun _ ->
    NuGet (fun p ->
    { p with
        Version = version
        OutputPath = artifactsDir
        WorkingDir = artifactsDir
        Properties = ["Configuration","Release"]
    }) "SemanticReleaseNotesParser.Fake.nuspec"
)

Target "All" DoNothing

// Dependencies
"Clean" ==> "ChocoPack"

"Clean"
  ==> "RestorePackages"
  =?> ("PrepareSourceLink", Choco.IsAvailable)
  ==> "BuildApp"
  =?> ("InspectCodeAnalysis", Choco.IsAvailable)
  =?> ("ILRepack", Choco.IsAvailable)
  =?> ("SourceLink", Choco.IsAvailable)
  ==> "BuildReleaseNotes"
  ==> "BuildTest"
  ==> "Test"
  =?> ("Zip", buildServer <> Travis)
  =?> ("PackFakeHelper", buildServer <> Travis)
  =?> ("ChocoPack", Choco.IsAvailable)
  =?> ("NugetPackCore", buildServer <> Travis)
  =?> ("NugetPackFake", buildServer <> Travis)
  ==> "All"

// start build
RunTargetOrDefault "All"
