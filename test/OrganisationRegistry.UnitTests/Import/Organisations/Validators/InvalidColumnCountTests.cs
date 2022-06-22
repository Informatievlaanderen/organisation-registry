namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using Api.HostedServices.ProcessImportedFiles.Validation;
using CsvHelper;
using FluentAssertions;
using Moq;
using Xunit;

public class InvalidColumnCountTests
{
    [Theory]
    [InlineData(1,2)]
    [InlineData(5,9)]
    [InlineData(7,3)]
    public void ReturnsIssue_WhenHeaderAndRowHaveDifferentCount(int rowCount, int headerRowCount)
    {
        var readerRow = GetReaderRow(rowCount, headerRowCount);

        var issue = InvalidColumnCount.Validate(readerRow);

        issue.Should().BeEquivalentTo(new ValidationIssue(1, InvalidColumnCount.FormatMessage()));
    }

    [Theory]
    [InlineData(1,1)]
    [InlineData(5,5)]
    [InlineData(7,7)]
    public void ReturnsEmpty_WhenHeaderAndRowHaveSameCount(int rowCount, int headerRowCount)
    {
        var readerRow = GetReaderRow(rowCount, headerRowCount);

        var issue = InvalidColumnCount.Validate(readerRow);

        issue.Should().BeNull();
    }

    private static IReaderRow GetReaderRow(int rowCount, int headerRowCount)
    {
        var readerRowMock = new Mock<IReaderRow>();
        readerRowMock.SetupGet(rr => rr.HeaderRecord).Returns(new string[headerRowCount]);
        readerRowMock.SetupGet(rr => rr.Parser).Returns(GetParser(rowCount));
        return readerRowMock.Object;
    }

    private static IParser GetParser(int rowCount)
    {
        var parserMock = new Mock<IParser>();
        parserMock.SetupGet(p => p.Record).Returns(new string[rowCount]);
        parserMock.SetupGet(p => p.Row).Returns(1);
        return parserMock.Object;
    }
}
