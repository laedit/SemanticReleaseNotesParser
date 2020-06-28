$packageName = 'SemanticReleaseNotesParser'
$url = 'https://github.com/laedit/SemanticReleaseNotesParser/releases/download/v[version]/SemanticReleaseNotesParser.[version].zip'

Install-ChocolateyZipPackage "$packageName" "$url" "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
