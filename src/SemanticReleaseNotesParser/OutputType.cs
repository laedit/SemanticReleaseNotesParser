using System;

namespace SemanticReleaseNotesParser
{
    [Flags]
    internal enum OutputType
    {
        File = 1,
        Environment = 2,
        FileAndEnvironment = File | Environment
    }
}
