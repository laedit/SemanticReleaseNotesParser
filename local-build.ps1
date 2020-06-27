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
dotnet test src --no-build --nologo --verbosity normal --collect:"XPlat Code Coverage"
dotnet reportgenerator "-reports:src/**/TestResults/**/coverage.cobertura.xml" "-targetdir:artifacts/reports" -reporttypes:Html

# Build release notes
./publish/SemanticReleaseNotesParser.exe -g=categories --debug -o="artifacts/ReleaseNotes.html" --pluralizecategoriestitle --includestyle
./publish/SemanticReleaseNotesParser.exe -g=categories --debug -t=environment -f=markdown --pluralizecategoriestitle # for github release?

# Publish
Copy-Item -Path "src/SemanticReleaseNotesParser/bin/Release/SemanticReleaseNotesParser.$version.nupkg" -Destination artifacts
Copy-Item -Path "src/SemanticReleaseNotesParser.Core/bin/Release/SemanticReleaseNotesParser.Core.$version.nupkg" -Destination artifacts
Copy-Item -Path "src/SemanticReleaseNotesParser.Core/bin/Release/SemanticReleaseNotesParser.Core.$version.snupkg" -Destination artifacts
dotnet publish src/SemanticReleaseNotesParser --configuration Release --output ./publish --self-contained true --runtime win-x86 -p:PublishSingleFile=true -p:PublishTrimmed=true /p:PackAsTool=false /p:Version=$version
Copy-Item -Path "artifacts/ReleaseNotes.html" -Destination publish
Compress-Archive -Path publish/* -DestinationPath "artifacts/SemanticReleaseNotesParser.$version.zip"

if (choco -v) {
    choco pack --version $version
}
