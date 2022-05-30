namespace OrganisationRegistry;

using System;

public abstract class DomainException : Exception
{
    protected DomainException() { }

    protected DomainException(string message) : base(message) { }

    protected DomainException(string message, Exception inner) : base(message, inner) { }
}