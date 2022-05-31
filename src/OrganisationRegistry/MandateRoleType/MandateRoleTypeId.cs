namespace OrganisationRegistry.MandateRoleType;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class MandateRoleTypeId : GuidValueObject<MandateRoleTypeId>
{
    public MandateRoleTypeId([JsonProperty("id")] Guid mandateRoleTypeId) : base(mandateRoleTypeId) { }
}
