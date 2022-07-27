namespace OrganisationRegistry.Api.Backoffice.Parameters.ContactType.Requests;

using System;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

/// <summary>
/// validates the validity of a regex
/// </summary>
/// <typeparam name="T">the type of the object containing a property that is a regex</typeparam>
public class RegexValidator<T> : IPropertyValidator<T, string?>
{
    public bool IsValid(ValidationContext<T> context, string? value)
    {
        try
        {
            if (value is { })
                // ReSharper disable once ObjectCreationAsStatement
                new Regex(value);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string GetDefaultMessageTemplate(string errorCode)
        => "Regular expression is invalid.";

    public string Name
        => "Regular expression validator";
}
