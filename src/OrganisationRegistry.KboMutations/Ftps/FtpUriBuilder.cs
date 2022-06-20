namespace OrganisationRegistry.KboMutations.Ftps;

using System;
using System.Linq;

public class FtpUriBuilder : UriBuilder
{
    private const string FtpScheme = "ftp";

    public string FileName => Uri.Segments.Last();

    public FtpUriBuilder(string host, int port)
        : this(FtpScheme, host, port) { }

    private FtpUriBuilder(string scheme, string host, int port)
        : base(scheme, host, port) { }

    public FtpUriBuilder AppendDir(string sourcePath)
        => new(
            Scheme,
            Host,
            Port)
        {
            Path = $"{Path}{sourcePath.Trim('/')}/",
        };

    public FtpUriBuilder WithPath(string fullPath)
        => new(
            Scheme,
            Host,
            Port)
        {
            Path = fullPath,
        };

    public FtpUriBuilder AppendFileName(string fileName)
        => new(
            Scheme,
            Host,
            Port)
        {
            Path = $"{Path}{fileName}",
        };
}
