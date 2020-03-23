namespace OrganisationRegistry.Organisation
{
    using System;
    using IbanBic;

    public class BankAccountBic
    {
        public static BankAccountBic CreateWithExpectedValidity(string bic, bool isValidBic)
        {
            return new BankAccountBic(bic, isValidBic);
        }

        public static BankAccountBic CreateWithUnknownValidity(string bic)
        {
            var isValidBic = BicUtils.IsValid(bic, out _);
            return new BankAccountBic(bic, isValidBic);
        }

        public string Bic { get; }
        public bool IsValidBic { get; }

        private BankAccountBic(
            string bic,
            bool isValidBic)
        {
            IsValidBic = isValidBic;
            Bic = bic ?? string.Empty;

            if (isValidBic)
                ValidateBicOrThrow();
        }

        private void ValidateBicOrThrow()
        {
            try
            {
                BicUtils.ValidateBIC(Bic);
            }
            catch (Exception ex)
            {
                throw new InvalidIbanException(ex);
            }
        }
    }
}
