[![Build status](https://ci.appveyor.com/api/projects/status/6h723a3g2e99r6on?svg=true)](https://ci.appveyor.com/project/laedit/semanticreleasenotesparser) [![Coverage Status](https://coveralls.io/repos/laedit/SemanticReleaseNotesParser/badge.svg?branch=master)](https://coveralls.io/r/laedit/SemanticReleaseNotesParser?branch=master)
 

![Project icon](icon.png)

Parser for [Semantic Release Notes](http://www.semanticreleasenotes.org/) v0.3.

Can be used to parse a semantic release notes and to format it to a markdown or html file or environment variable, for use on build sever.

## Install
 - Nuget (core library) (soon)
 - Chocolatey (command line tool) (soon)

## Usage
### CommandLine
`SemanticReleaseNotes`: parse the default `ReleaseNotes.md` and generates a `ReleaseNotes.html`
 
Parameters:

 - `-r=[filename]`: Release notes file path to parse (default: ReleaseNotes.md)
 - `-o=[filename]`: Path of the resulting file (default: ReleaseNotes.html
 - `-t=[file|environment]`: Type of output (default: file)
 - `-f=[Html|Markdown]`: Format of the resulting file [(default: Html)
 - `--template`: Path of the liquid template file to format the result ; Overrides type and format of output
 - `--debug`: add debug messages
 - `-h`: help

### Library
- `SemanticReleaseNotesParser` class parse a file and produces a `ReleaseNotes`
- `SemanticReleaseNotesFormatter` class format a `ReleaseNotes`

Icon: [Article]([http://thenounproject.com/term/article/16591/](http://thenounproject.com/term/article/16591/)) designed by [Stefan Parnarov]([http://thenounproject.com/sapi/](http://thenounproject.com/sapi/)) from The Noun Project.