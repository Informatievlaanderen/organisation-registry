namespace OrganisationRegistry.UnitTests.Import.Organisations.Create;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Import;
using Tests.Shared.TestDataBuilders;
using Xunit;

public class DeserializedRecordTests
{
    [Fact]
    public void WhenGivenADeserializedRecord_ItProducesACorrectCommandItem()
    {
        var fixture = new Fixture();

        var label1Id = fixture.Create<Guid>();
        const string label1 = "label 1";

        var label2Id = fixture.Create<Guid>();
        const string label2 = "label 2";

        var labelTypes = new Dictionary<string, (Guid id, string name)>
        {
            { label1, (id: label1Id, name: label1) },
            { label2, (id: label2Id, name: label2) },
        };

        var random = new Random();
        var expectedArticle = Article.All[random.NextInt64(0, 3)];
        var expectedName = fixture.Create<string>();
        var expectedParent = fixture.Create<string>();
        var expectedReference = fixture.Create<string>();
        var expectedShortName = fixture.Create<string>();
        var expectedValidityStart = fixture.Create<DateTime>();
        var expectedOperationalValidityStart = fixture.Create<DateTime>();

        var expectedLabel1Value = fixture.Create<string>();
        var expectedLabel2Value = fixture.Create<string>();

        var deserializedRecord = new CreateOrganisationsDeserializedRecordBuilder(expectedReference, expectedName)
            .WithArticle(expectedArticle)
            .WithParent(fixture.Create<string>())
            .WithShortName(expectedShortName)
            .WithValidityStart(expectedValidityStart.ToString("yyyy-MM-dd"))
            .WithOperationalValidityStart(expectedOperationalValidityStart.ToString("yyyy-MM-dd"))
            .AddLabel(label1, expectedLabel1Value)
            .AddLabel(label2, expectedLabel2Value)
            .Build();

        var commandItem = deserializedRecord.ToCommandItem(labelTypes, expectedParent, sortOrder: 1);

        commandItem.Article.Should().Be(expectedArticle);
        commandItem.Name.Should().Be(expectedName);
        commandItem.ParentIdentifier.Should().Be((OrganisationParentIdentifier)expectedParent);
        commandItem.Reference.Should().Be(expectedReference);
        commandItem.ShortName.Should().Be(expectedShortName);
        commandItem.Validity_Start.Should().Be(DateOnly.FromDateTime(expectedValidityStart));
        commandItem.OperationalValidity_Start.Should().Be(DateOnly.FromDateTime(expectedOperationalValidityStart));

        commandItem.Labels.Should().BeEquivalentTo(ImmutableList.Create(new Label(label1Id, label1, expectedLabel1Value), new Label(label2Id, label2, expectedLabel2Value)));
    }
}
