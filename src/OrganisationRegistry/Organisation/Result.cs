namespace OrganisationRegistry.Organisation;

using System;
using System.Linq;

public class Result<T>
{
    private readonly T? _maybeValue;
    public string[] ErrorMessages { get; }

    public T Value
    {
        get
        {
            if (_maybeValue is { } value)
                return value;

            throw new NullReferenceException("Result has no value when it was constructed with Fail()");
        }
    }

    public bool HasErrors => ErrorMessages.Any();

    private Result(T value)
    {
        ErrorMessages = Array.Empty<string>();
        _maybeValue = value;
    }

    private Result(string[] errorMessages)
        => ErrorMessages = errorMessages;

    public static Result<T> Fail(params string[] errorMessages)
        => new(errorMessages);

    public static Result<T> Success(T value)
        => new(value);
}
