$packageName = 'SemanticReleaseNotesParser'
$url = 'https://github.com/laedit/SemanticReleaseNotesParser/releases/download/v[version]/SemanticReleaseNotesParser.win-x86.[version].zip'

Install-ChocolateyZipPackage "$packageName" "$url" "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
