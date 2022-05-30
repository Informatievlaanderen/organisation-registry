namespace OrganisationRegistry.KboMutations.UnitTests;

using FluentAssertions;
using Ftps;
using Xunit;

public class FtpUriBuilderTests
{
    [Fact]
    public void BuildsToString()
    {
        var sut = new FtpUriBuilder("test.be", 23);

        sut.ToString().Should().Be("ftp://test.be:23/");
    }

    [Fact]
    public void AppendsDir()
    {
        var sut =
            new FtpUriBuilder("test.be", 23)
                .AppendDir("from_vip");

        sut.ToString().Should().Be("ftp://test.be:23/from_vip/");
    }

    [Fact]
    public void TrimsBeginWhenAppendingDir()
    {
        var sut =
            new FtpUriBuilder("test.be", 23)
                .AppendDir("/from_vip");

        sut.ToString().Should().Be("ftp://test.be:23/from_vip/");
    }

    [Fact]
    public void TrimsBeginWhenAppendingConcatenatedDir()
    {
        var sut =
            new FtpUriBuilder("test.be", 23)
                .AppendDir("/from_vip/cache");

        sut.ToString().Should().Be("ftp://test.be:23/from_vip/cache/");
    }

    [Fact]
    public void AppendsSeveralDirs()
    {
        var sut =
            new FtpUriBuilder("test.be", 23)
                .AppendDir("cache")
                .AppendDir("from_vip");

        sut.ToString().Should().Be("ftp://test.be:23/cache/from_vip/");
    }

    [Fact]
    public void AppendsFileName()
    {
        var sut =
            new FtpUriBuilder("test.be", 23)
                .AppendDir("cache")
                .AppendDir("from_vip")
                .AppendFileName("test.csv");

        sut.Should().Be("ftp://test.be:23/cache/from_vip/test.csv");
    }

    [Fact]
    public void AppendsFullPath()
    {
        var sut =
            new FtpUriBuilder("test.be", 23)
                .WithPath("/from_vip/test.csv");

        sut.ToString().Should().Be("ftp://test.be:23/from_vip/test.csv");
    }

    [Fact]
    public void AppendsFullPathOverwritesPreviousPath()
    {
        var sut =
            new FtpUriBuilder("test.be", 23)
                .AppendDir("cache")
                .WithPath("/from_vip/test.csv");

        sut.ToString().Should().Be("ftp://test.be:23/from_vip/test.csv");
    }
}