namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Text.RegularExpressions;
    using IbanBic;

    public class BankAccountNumber
    {
        public static BankAccountNumber CreateWithExpectedValidity(string bankAccountNumber, bool isValidIban)
        {
            var accountNumber = new BankAccountNumber(bankAccountNumber, isValidIban);

            if (isValidIban)
                accountNumber.ValidateIbanOrThrow();

            return accountNumber;
        }

        public static BankAccountNumber CreateWithUnknownValidity(string bankAccountNumber)
        {
            var isValid = IbanUtils.IsValid(CleanBankAccountNumber(bankAccountNumber), out _);
            return new BankAccountNumber(bankAccountNumber, isValid);
        }

        public string Number { get; }
        public bool IsValidIban { get; }

        private BankAccountNumber(
            string bankAccountNumber,
            bool isValidIban)
        {
            IsValidIban = isValidIban;
            Number = isValidIban ? CleanBankAccountNumber(bankAccountNumber) : bankAccountNumber ?? string.Empty;
        }

        private BankAccountNumber(string bankAccountNumber)
        {
            Number = bankAccountNumber ?? string.Empty;
        }

        private static string CleanBankAccountNumber(string bankAccountNumber)
            => Regex.Replace(bankAccountNumber, @"[^0-9a-zA-Z]", "");

        private void ValidateIbanOrThrow()
        {
            try
            {
                IbanUtils.Validate(Number);
            }
            catch (Exception ex)
            {
                throw new InvalidIbanException(ex);
            }
        }
    }
}
