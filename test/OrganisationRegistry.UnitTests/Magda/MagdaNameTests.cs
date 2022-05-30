namespace OrganisationRegistry.UnitTests.Magda;

using Api.Infrastructure.Magda;
using FluentAssertions;
using global::Magda.GeefOnderneming;
using Xunit;

public class MagdaNameTests
{
    [Fact]
    public void TrimsName()
    {
        var name = new MagdaOrganisationResponse.Name(new[]
        {
            new NaamOndernemingType
            {
                Naam = " Test "
            }
        });

        name.Value.Should().Be("Test");
    }
}