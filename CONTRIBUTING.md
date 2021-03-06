# Contributing to Semantic Release Notes Parser

## Getting started

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)

Once you're familiar with Git and GitHub, clone the repository and run the SemanticReleaseNotesParser.sln to start coding.

## Discussing ideas 

* [Gitter Chatroom](https://gitter.im/laedit/SemanticReleaseNotesParser)
* [GitHub Issues](https://github.com/laedit/SemanticReleaseNotesParser/issues)

## Coding conventions

We are following as much as possible the [C# coding conventions](https://msdn.microsoft.com/en-us/library/ff926074.aspx).  
We prefer spaces over tab for indentation.  
We have an [editorconfig](http://EditorConfig.org) [file](./.editorconfig) if you use an editor or plugin respecting it.

## Testing

Don't forget to add some tests for your functionality in the tests suit.  
You can see the result either:
- in Visual Studio or other IDE supporting xUnit2
- on [AppVeyor](https://ci.appveyor.com/project/laedit/SemanticReleaseNotesParser) and [Travis](https://travis-ci.org/laedit/SemanticReleaseNotesParser) after the PR is submitted
- run build.bat or build.sh