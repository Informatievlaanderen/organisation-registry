namespace OrganisationRegistry;

using System;
using System.Collections.Immutable;

public abstract class CombinedException<T> : DomainException
{
    public ImmutableList<(Exception ex, T context)> Exceptions { get; private set; } = ImmutableList<(Exception, T)>.Empty;

    public CombinedException() : base("Een of meerdere fouten kwam(en) voor")
    {
    }

    public void Add(DomainException ex, T context)
    {
        Exceptions = Exceptions.Add((ex, context));
    }
}

public class CreateOrganisationsImportException : CombinedException<string>
{ }

public class TerminateOrganisationsImportException : CombinedException<string>
{ }
