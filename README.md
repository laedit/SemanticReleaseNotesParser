[![Licence Apache 2](https://img.shields.io/badge/licence-Apache%202-blue.svg)](https://github.com/laedit/vika/blob/master/LICENSE) 
[![Windows Build status](https://ci.appveyor.com/api/projects/status/6h723a3g2e99r6on?svg=true)](https://ci.appveyor.com/project/laedit/semanticreleasenotesparser) 
[![Linux Build Status](https://travis-ci.org/laedit/SemanticReleaseNotesParser.svg?branch=master)](https://travis-ci.org/laedit/SemanticReleaseNotesParser)
[![codecov.io](https://codecov.io/github/laedit/SemanticReleaseNotesParser/coverage.svg?branch=master)](https://codecov.io/github/laedit/SemanticReleaseNotesParser?branch=master)
[![SourceLink](https://img.shields.io/badge/SourceLink-ready-brightgreen.svg)](https://ctaggart.github.io/SourceLink/index.html)


![Project icon](icon.png)

Parser for [Semantic Release Notes](http://www.semanticreleasenotes.org/) ([fallback](http://htmlpreview.github.io/?https://github.com/avanderhoorn/Semantic-Release-Notes/blob/master/index.html)) v0.3.

Can be used to parse a semantic release notes and to format it to a markdown or html file or environment variable, for use on build sever.

## Install
 - [Nuget (core library)](https://www.nuget.org/packages/SemanticReleaseNotesParser.Core/): `Install-Package SemanticReleaseNotesParser.Core`
 - [Chocolatey (command line tool)](https://chocolatey.org/packages/semanticreleasenotesparser/): `choco install semanticreleasenotesparser`
 - [Manual (command line tool)](https://github.com/laedit/SemanticReleaseNotesParser/releases): download the zip

## Usage
### CommandLine
`SemanticReleaseNotes [parameters]`: parse the default `ReleaseNotes.md` and generates a `ReleaseNotes.html`

All parameters are optional.

Parameters:

 - `-r=<filename>`: Release notes file path to parse (default: `ReleaseNotes.md`)
 - `-o=<filename>`: Path of the resulting file (default: `ReleaseNotes.html`)
 - `-t=<file|environment|fileandenvironment>`: Type of output (default: `file`)
   - `file` generates a file with the name from `-o` or `ReleaseNotes.html` by default
   - `environment` set an environment variable with the name `SemanticReleaseNotes` and value of the formatted release notes; support [build servers](https://github.com/laedit/SemanticReleaseNotesParser/wiki/Build-Servers-Support)
   - `fileandenvironment` make both of the above
 - `-f=<html|markdown>`: Format of the resulting file (default: `html`)
 - `-g=<sections|categories>`: Defines the grouping of items (default: `sections`)
 - `--template`: Path of the [liquid template](https://github.com/laedit/SemanticReleaseNotesParser/wiki/Format-templating) file to format the result ; Overrides type, format and groupby of output
 - `--pluralizecategoriestitle`: Pluralize categories title; works only with `g=categories`
 - `--includestyle[=<custom style>]`: Include style in the html output; if no custom style is provided, the default is used
 - `--debug`: Add debug messages
 - `-h`: Help

### Library
- `SemanticReleaseNotesConverter` class help to parse, format or convert a `ReleaseNotes`

## Third party libraries
 - [DotLiquid](https://github.com/dotliquid/dotliquid) is used to process the liquid templates
 - [CommonMark.NET](https://github.com/Knagis/CommonMark.NET) is used to process the markdown
 - [Humanizer](https://github.com/Humanizr/Humanizer) is used to titleize and pluralize categories title

# Contribute

Please pick an existing issue, or create one in order to discuss it before you go full ahead.

## Build
Just use [`local-build.ps1`](./local-build.ps1).  
It will:
 - apply the source format
 - build the projects
 - launch the tests
 - packages all the thing


## Release
1. Generates release notes with https://github.com/StefH/GitHubReleaseNotes or https://github.com/GitTools/GitReleaseNotes [TODO]
2. Create and push a new tag based on the template: `v[x].[y].[y]` => `v0.1.0`

Icon: [Article](https://thenounproject.com/term/article/16591/) designed by [Stefan Parnarov](https://thenounproject.com/sapi/) from The Noun Project.
