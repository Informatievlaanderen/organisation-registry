﻿namespace OrganisationRegistry.Api.LifecyclePhaseType
{
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.LifecyclePhaseType;

    public class OnlyOneDefaultLifecyclePhaseTypeTypeValidator : IOnlyOneDefaultLifecyclePhaseTypeValidator
    {
        private readonly OrganisationRegistryContext _context;

        public OnlyOneDefaultLifecyclePhaseTypeTypeValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor, LifecyclePhaseTypeStatus lifecyclePhaseTypeStatus)
        {
            if (lifecyclePhaseTypeStatus == LifecyclePhaseTypeStatus.NonDefault)
                return false;

            return _context.LifecyclePhaseTypeList.Any(item =>
                item.RepresentsActivePhase == (lifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase) &&
                item.IsDefaultPhase);
        }

        public bool ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(LifecyclePhaseTypeId lifecyclePhaseTypeId, LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor, LifecyclePhaseTypeStatus lifecyclePhaseTypeStatus)
        {
            if (lifecyclePhaseTypeStatus == LifecyclePhaseTypeStatus.NonDefault)
                return false;

            return _context.LifecyclePhaseTypeList
                .AsQueryable()
                .Where(item => item.Id != lifecyclePhaseTypeId)
                .Any(item =>
                    item.RepresentsActivePhase == (lifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase) &&
                    item.IsDefaultPhase);
        }
    }
}
