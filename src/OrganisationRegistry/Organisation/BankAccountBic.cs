namespace OrganisationRegistry.Organisation
{
    using System;
    using IbanBic;

    public class BankAccountBic
    {
        public string Bic { get; }
        public bool IsValidBic { get; }

        public BankAccountBic(
            string bic,
            bool isValidBic)
        {
            IsValidBic = isValidBic;
            Bic = bic;

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
