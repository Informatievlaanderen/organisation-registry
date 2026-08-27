namespace OrganisationRegistry.Acl;

using System.Collections.Generic;
using Internals;
using OrganisationRegistry.Acl.Impl;
using OrganisationRegistry.Infrastructure.Authorization;

public static class Verify
{
    public static void Run()
    {
        var registry = new Registry(new Dictionary<string, Capability>
        {
            ["/organisations/capacities"] = OrganisationCapabilities.CanViewCapacities,
            ["AC"] = OrganisationCapabilities.CanManageChildren
        });

        var builder = new AclRuntimeBuilder<OrganisationData>(new OrganisationProvider());


        var arg = new OrganisationData(CrudOperation.Write, "OVO001", "OVO999", ["OVO001"], IsVlimpersActiveForTargetOrganisation: false);
        Rule(
            "Algemeen beheerder requests Update",
            Role.AlgemeenBeheerder,
            arg,
            expectedPass: true);

        Rule(
            "Decentraal beheerder requests Update on an unrelated org",
            Role.DecentraalBeheerder,
            new OrganisationData(CrudOperation.Write, "OVO001", "OVO999", ["OVO001"], IsVlimpersActiveForTargetOrganisation: false),
            expectedPass: false);

        Rule(
            "Vlimpers beheerder requests Read, Vlimpers active for target org",
            Role.VlimpersBeheerder,
            new OrganisationData(CrudOperation.Read, "OVO001", "OVO999", ["OVO001"], IsVlimpersActiveForTargetOrganisation: true),
            expectedPass: true);



        void Rule(string label, Role role, OrganisationData data, bool expectedPass)
        {
            var capability = registry.Resolve("/organisations/capacities");
            var aclRuntime = builder.Build(role, capability, data);
            var result = AclHost.Exec(aclRuntime);
            var mark = result.IsSuccess == expectedPass ? "OK " : "MISMATCH";

        }
    }
}
