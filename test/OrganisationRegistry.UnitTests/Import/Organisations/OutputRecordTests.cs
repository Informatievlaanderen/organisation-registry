namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Import;
using Xunit;

public class OutputRecordTests
{
    [Fact]
    public void WhenGivenADeserializedRecord_ItProducesACorrectOutputRecord()
    {
        var fixture = new Fixture();

        const string labelprefix = "label#";

        var label1Id = fixture.Create<Guid>();
        const string label1 = "label 1";

        var label2Id = fixture.Create<Guid>();
        const string label2 = "label 2";

        var labelTypes = new Dictionary<string, Guid>
        {
            { label1, label1Id },
            { label2, label2Id },
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

        var deserializedRecord = new DeserializedRecord
        {
            Article = Field.FromValue(ColumnNames.Article, expectedArticle),
            Name = Field.FromValue(ColumnNames.Name, expectedName),
            Parent = Field.FromValue(ColumnNames.Parent, fixture.Create<string>()),
            Reference = Field.FromValue(ColumnNames.Reference, expectedReference),
            ShortName = Field.FromValue(ColumnNames.ShortName, expectedShortName),
            Validity_Start = Field.FromValue(ColumnNames.Validity_Start, expectedValidityStart.ToString("yyyy-MM-dd")),
            OperationalValidity_Start = Field.FromValue(ColumnNames.OperationalValidity_Start, expectedOperationalValidityStart.ToString("yyyy-MM-dd")),
            Labels = ImmutableList.Create(Field.FromValue($"{labelprefix}{label1}", expectedLabel1Value), Field.FromValue($"{labelprefix}{label2}", expectedLabel2Value)),
        };

        var outputRecord = OutputRecord.From(labelTypes, deserializedRecord, expectedParent, 1);

        outputRecord.Article.Should().Be(expectedArticle);
        outputRecord.Name.Should().Be(expectedName);
        outputRecord.ParentIdentifier.Should().Be((OrganisationParentIdentifier)expectedParent);
        outputRecord.Reference.Should().Be(expectedReference);
        outputRecord.ShortName.Should().Be(expectedShortName);
        outputRecord.Validity_Start.Should().Be(DateOnly.FromDateTime(expectedValidityStart));
        outputRecord.OperationalValidity_Start.Should().Be(DateOnly.FromDateTime(expectedOperationalValidityStart));

        outputRecord.Labels.Should().BeEquivalentTo(ImmutableList.Create(new Label(label1Id, label1, expectedLabel1Value), new Label(label2Id, label2, expectedLabel2Value)));
    }
}
