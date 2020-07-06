using System;

namespace SemanticReleaseNotesParser.Abstractions
{
    internal interface IWebClient : IDisposable
    {
        byte[] UploadData(string address, string method, byte[] data);
    }
}
