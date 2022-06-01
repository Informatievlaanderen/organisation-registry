namespace OrganisationRegistry.Api.Infrastructure;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

[AttributeUsage(AttributeTargets.Property)]
public sealed class OrderAttribute : Attribute
{
    public OrderAttribute([CallerLineNumber]int order = 0)
    {
        Order = order;
    }

    public int Order { get; }
}

[AttributeUsage(AttributeTargets.Property)]
public class ExcludeFromCsvAttribute : Attribute
{
}

/// <summary>
/// Original code taken from
/// http://www.tugberkugurlu.com/archive/creating-custom-csvmediatypeformatter-in-asp-net-web-api-for-comma-separated-values-csv-format
/// Adapted for ASP.NET Core and uses ; instead of , for delimiters
/// </summary>
public class CsvOutputFormatter : OutputFormatter
{
    private readonly CsvFormatterOptions _options;

    public string ContentType { get; }

    public CsvOutputFormatter(CsvFormatterOptions csvFormatterOptions)
    {
        ContentType = "text/csv";
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));

        _options = csvFormatterOptions ?? throw new ArgumentNullException(nameof(csvFormatterOptions));

        //SupportedEncodings.Add(Encoding.GetEncoding("utf-8"));
    }

    protected override bool CanWriteType(Type? type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        return IsTypeOfIEnumerable(type);
    }

    private static bool IsTypeOfIEnumerable(Type type)
    {
        return type.GetInterfaces().Any(interfaceType => interfaceType == typeof(IList));
    }

    private static IEnumerable<PropertyInfo> GetOrderedProperties(Type type)
    {
        var lookup = new Dictionary<Type, int>();

        var count = 0;
        lookup[type] = count++;
        var parent = type.BaseType;
        while (parent != null)
        {
            lookup[parent] = count;
            count++;
            parent = parent.BaseType;
        }

        return type.GetProperties().OrderByDescending(prop => prop.DeclaringType is { } declaringType ? lookup[declaringType] : int.MaxValue);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var response = context.HttpContext.Response;

        var type = context.Object!.GetType();

        var itemType = type.GetGenericArguments().Length > 0
            ? type.GetGenericArguments()[0]
            : type.GetElementType();

        var stringWriter = new StringWriter();

        if (_options.UseSingleLineHeaderInCsv)
            stringWriter.WriteLine(string.Join<string>(
                _options.CsvDelimiter,
                GetOrderedProperties(itemType!)
                    .Where(pi => pi.GetCustomAttribute<ExcludeFromCsvAttribute>() == null)
                    .Select(pi => pi.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                                  ?? pi.GetCustomAttribute<DisplayAttribute>()?.Name
                                  ?? pi.Name)));

        foreach (var obj in (IEnumerable<object>) context.Object)
        {
            var vals =
                GetOrderedProperties(obj.GetType())
                    .Where(pi => pi.GetCustomAttribute<ExcludeFromCsvAttribute>() == null)
                    .Select(pi => new { Value = pi.GetValue(obj, null) });

            var valueLine = string.Empty;

            foreach (var val in vals)
            {
                if (val.Value != null)
                {
                    var tempVal = val.Value.ToString();

                    //Check if the value contans a comma and place it in quotes if so
                    if (tempVal!.Contains(_options.CsvDelimiter))
                        tempVal = string.Concat("\"", tempVal, "\"");

                    //Replace any \r or \n special characters from a new line with a space
                    if (tempVal.Contains("\r"))
                        tempVal = tempVal.Replace("\r", " ");
                    if (tempVal.Contains("\n"))
                        tempVal = tempVal.Replace("\n", " ");

                    valueLine = string.Concat(valueLine, tempVal, _options.CsvDelimiter);
                }
                else
                {

                    valueLine = string.Concat(valueLine, string.Empty, _options.CsvDelimiter);
                }
            }

            stringWriter.WriteLine(valueLine.TrimEnd(_options.CsvDelimiter.ToCharArray()));
        }

        var streamWriter = new StreamWriter(response.Body);
        await streamWriter.WriteAsync(stringWriter.ToString());
        await streamWriter.FlushAsync();
    }
}
