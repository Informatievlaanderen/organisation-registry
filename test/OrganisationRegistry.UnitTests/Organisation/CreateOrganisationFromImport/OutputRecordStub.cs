namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisationFromImport;

using System;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Import;

public class OutputRecordStub : OutputRecord
{
    public OutputRecordStub(string reference = "", Guid parentOrganisationId = default, string name = "", int sortOrder = 0) : base(reference, parentOrganisationId, name, sortOrder)
    {
    }

    public new DateOnly? Validity_Start
    {
        get => base.Validity_Start;
        init => base.Validity_Start = value;
    }
    public new string? ShortName
    {
        get => base.ShortName;
        init => base.ShortName = value;
    }
    public new Article? Article
    {
        get => base.Article;
        init => base.Article = value;
    }
    public new DateOnly? OperationalValidity_Start
    {
        get => base.OperationalValidity_Start;
        init => base.OperationalValidity_Start = value;
    }
}
