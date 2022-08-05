namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

public class OrganisatieRegisterCsvWriter
{
    private const string NewLine = "\r\n";

    public static string WriteCsv<T>(Type type, Func<T, object> createRecord, IEnumerable<T> items)
    {
        var stringWriter = new StringWriter();
        var csvWriter = new CsvWriter(
            stringWriter,
            new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", NewLine = NewLine });

        csvWriter.WriteHeader(type);
        csvWriter.NextRecord();
        foreach (var item in items)
        {
            var record = createRecord(item);
            csvWriter.WriteRecord(record); BREAKING THE BUILD

            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        return stringWriter.ToString();
    }
}
