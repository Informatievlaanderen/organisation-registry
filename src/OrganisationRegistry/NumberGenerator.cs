namespace OrganisationRegistry;

using System;

public abstract class NumberGenerator
{
    private readonly string _prefix;
    private readonly string _name;
    private readonly Func<string?> _maxNumberFunc;
    private int? _lastNumber;

    protected NumberGenerator(
        string prefix,
        string name,
        Func<string?> maxNumberFunc)
    {
        _prefix = prefix;
        _name = name;
        _maxNumberFunc = maxNumberFunc;
    }

    public string GenerateNumber()
    {
        var maxNumber = _maxNumberFunc();
        if (string.IsNullOrWhiteSpace(maxNumber))
            maxNumber = "0";

        if (!int.TryParse(maxNumber.Replace(_prefix, ""), out var number))
            throw new InvalidOperationException($"{_name} moet een integer getal zijn");

        _lastNumber = number + 1;
        return $"{_prefix}{_lastNumber:D6}";
    }

    public string GenerateNextNumber()
        => _lastNumber is { }
            ? $"{_prefix}{++_lastNumber:D6}"
            : GenerateNumber();
}
