namespace OrganisationRegistry.KboMutations.UnitTests;

using System.Collections.Generic;
using FluentAssertions;
using Ftps;
using Xunit;

public class FtpsListParserTests
{
    [Fact]
    public void ParsesInput()
    {
        var input =
            @"-rw-rw-r--   1 user user        0 Jun 13 17:49 pub_mut-ondernemingVKBO0200_20200613174810000.csv
-rw-rw-r--   1 user user       10 Jun 16 18:14 pub_mut-ondernemingVKBO0200_20200616181303000.csv
-rw-rw-r--   1 user user      100 Jun 12 17:41 test.csv";

        var baseUriBuilder = new FtpUriBuilder("test.be", 23);

        var ftpsListItems = FtpsListParser.Parse(baseUriBuilder, input);

        ftpsListItems.Should().ContainInOrder(new List<FtpsListItem>
        {
            new("pub_mut-ondernemingVKBO0200_20200613174810000.csv", "/pub_mut-ondernemingVKBO0200_20200613174810000.csv", "/",  "0"),
            new("pub_mut-ondernemingVKBO0200_20200616181303000.csv", "/pub_mut-ondernemingVKBO0200_20200616181303000.csv", "/", "10"),
            new("test.csv", "/test.csv", "/", "100"),
        });
    }
}
