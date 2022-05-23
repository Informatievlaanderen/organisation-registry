namespace OrganisationRegistry.UnitTests
{
    using System;

    public class UniqueOvoNumberValidatorStub: IUniqueOvoNumberValidator
    {
        private readonly bool _isOvoNumberTakenReturnValue;

        public UniqueOvoNumberValidatorStub(bool isOvoNumberTakenReturnValue)
            => _isOvoNumberTakenReturnValue = isOvoNumberTakenReturnValue;

        public bool IsOvoNumberTaken(string? ovoNumber)
            => _isOvoNumberTakenReturnValue;

        public bool IsOvoNumberTaken(Guid id, string ovoNumber)
            => _isOvoNumberTakenReturnValue;
    }
}
