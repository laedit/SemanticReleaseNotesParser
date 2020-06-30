If (Test-Path artifacts) {
    Remove-Item -Path artifacts -recurse
}
New-Item -ItemType directory -Path artifacts

# Install dependencies
dotnet tool restore
dotnet restore src

# Check format
dotnet format src

# Set version
$version = dotnet gitversion /output json /showvariable NuGetVersionV2
"Version: $version"

# Build
dotnet build src --configuration Release --no-restore /p:Version=$version

# Vika [TODO]
# vika --debug --includesource

# Test
dotnet test src --configuration Release --no-build --nologo --verbosity normal --collect:"XPlat Code Coverage"
dotnet reportgenerator "-reports:src/**/TestResults/**/coverage.cobertura.xml" "-targetdir:artifacts/reports" -reporttypes:Html

# Publish windows version
dotnet publish src/SemanticReleaseNotesParser --configuration Release --output ./publish --self-contained true --runtime win-x86 -p:PublishSingleFile=true -p:PublishTrimmed=true /p:PackAsTool=false /p:Version=$version

# Build release notes
./publish/SemanticReleaseNotesParser.exe -g=categories --debug -o="artifacts/ReleaseNotes.html" --pluralizecategoriestitle --includestyle

# Create packages
Copy-Item -Path "src/SemanticReleaseNotesParser/bin/Release/SemanticReleaseNotesParser.$version.nupkg" -Destination artifacts
Copy-Item -Path "src/SemanticReleaseNotesParser.Core/bin/Release/SemanticReleaseNotesParser.Core.$version.nupkg" -Destination artifacts
Copy-Item -Path "src/SemanticReleaseNotesParser.Core/bin/Release/SemanticReleaseNotesParser.Core.$version.snupkg" -Destination artifacts
Copy-Item -Path "artifacts/ReleaseNotes.html" -Destination publish
Compress-Archive -Path publish/* -DestinationPath "artifacts/SemanticReleaseNotesParser.$version.zip"

if (choco -v) {
    $installPath = "chocolatey/tools/chocolateyInstall.ps1"
    $originalContent = Get-Content $installPath
    $originalContent.replace('[version]', $version) | Set-Content $installPath
    choco pack chocolatey/semanticreleasenotes-parser.nuspec --version $version --outdir artifacts
    Set-Content $installPath $originalContent
}
