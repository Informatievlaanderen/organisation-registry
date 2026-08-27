namespace OrganisationRegistry.Acl.Internals;
using System;

[Flags]
public enum CrudOperation
{
    None = 0,
    Read   = 1 << 0, // 0001
    Create = 1 << 1, // 0010
    Write  = 1 << 2, // 0100
    Delete = 1 << 3, // 1000
}

public enum Tags
{
    Global,
    Organisation,
    Body,
    Person,
}
