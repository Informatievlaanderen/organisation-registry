/*
 * IBAN4Net
 * Copyright 2015 Vaclav Beca [sinkien]
 *
 * Based on Artur Mkrtchyan's project IBAN4j (https://github.com/arturmkrtchyan/iban4j).
 *
 *
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace OrganisationRegistry.IbanBic;

using System;
using System.Collections.Generic;
using System.Text;
using Exceptions;

/// <summary>
/// IBAN Utilities
/// </summary>
public static class IbanUtils
{
    private const int Mod = 97;
    private const long Max = 999999999;

    private const int Country_Code_Index = 0;
    private const int Country_Code_Length = 2;
    private const int Check_Digit_Index = Country_Code_Length;
    private const int Check_Digit_Length = 2;
    private const int Bban_Index = Check_Digit_Index + Check_Digit_Length;

    /// <summary>
    /// Validation of IBAN string
    /// </summary>
    /// <param name="iban">IBAN string</param>
    /// <exception cref="InvalidIbanFormat">Thrown when IBAN is invalid</exception>
    /// <exception cref="UnsupportedCountry">Thrown when INAB's country code is not supported</exception>
    /// <exception cref="InvalidCheckDigit">Thrown when IBAN string contains invalid check digit</exception>
    public static void Validate(string iban)
    {
        try
        {
            ValidateEmpty(iban);
            ValidateCountryCode(iban);
            ValidateCheckDigitPresence(iban);

            var structure = GetBbanStructure(iban);
            ValidateBbanLength(iban, structure);
            ValidateBbanEntries(iban, structure);

            ValidateCheckDigit(iban);
        }
        catch (InvalidCheckDigit)
        {
            throw;
        }
        catch (InvalidIbanFormat)
        {
            throw;
        }
        catch (UnsupportedCountry)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidIbanFormat(ex.Message, IbanFormatViolation.UNKNOWN, ex);
        }
    }

    /// <summary>
    /// Validation of IBAN string
    /// </summary>
    /// <param name="iban">Iban string</param>
    /// <param name="validationResult">Validation result</param>
    /// <returns>True if IBAN string is valid, false if it encounters any problem</returns>
    public static bool IsValid(string iban, out IbanFormatViolation validationResult)
    {
        var result = false;
        validationResult = IbanFormatViolation.NO_VIOLATION;

        if (string.IsNullOrEmpty(iban))
        {
            validationResult = IbanFormatViolation.IBAN_NOT_EMPTY_OR_NULL;
        }
        else
        {
            if (!HasValidCountryCode(iban, out validationResult)) return result;
            if (!HasValidCheckDigit(iban, out validationResult)) return result;
            var structure = GetBbanStructure(iban);
            if (!HasValidBbanLength(iban, structure, out validationResult)) return result;
            if (!HasValidBbanEntries(iban, structure, out validationResult)) return result;
            if (HasValidCheckDigitValue(iban, out validationResult)) result = true;
        }

        return result;
    }

    /// <summary>
    /// Checks whether country is supported.
    /// It is checked by trying to find the country code in defined BBAN structures.
    /// </summary>
    /// <param name="countryCode">Country code object</param>
    /// <returns>True if country code is supported, othewise false</returns>
    public static bool IsSupportedCountry(CountryCodeEntry? countryCode) => (CountryCode.GetCountryCode(countryCode?.Alpha2) != null) && (Bban.GetStructureForCountry(countryCode) != null);


    /// <summary>
    /// Checks whether country is supported.
    /// It is checked by trying to find the country code in defined BBAN structures.
    /// </summary>
    /// <param name="alpha2Code">Alpha2 code for country</param>
    /// <returns>True if country code is supported, othewise false</returns>
    public static bool IsSupportedCountry(string alpha2Code) => (CountryCode.GetCountryCode(alpha2Code) != null) && (Bban.GetStructureForCountry(alpha2Code) != null);

    /// <summary>
    /// Returns IBAN length for the specified country
    /// </summary>
    /// <param name="countryCode">Country code object</param>
    /// <returns>The length of IBAN for the specified country</returns>
    public static int GetIbanLength(CountryCodeEntry? countryCode)
    {
        var result = 0;
        var structure = GetBbanStructure(countryCode);

        if (structure != null)
        {
            result = Country_Code_Length + Check_Digit_Length + structure.GetBBanLength();
        }

        return result;
    }

    /// <summary>
    /// Calculates IBAN's check digit.
    /// ISO13616
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <returns>Check digit as string</returns>
    /// <exception cref="InvalidIbanFormat">Thrown if supplied iban string contains invalid chracters</exception>
    public static string CalculateCheckDigit(string iban)
    {
        string reformattedIban = ReplaceCheckDigit(iban, Iban.DEFAULT_CHECK_DIGIT);
        int modResult = CalculateMod(reformattedIban);
        int checkDigitValue = (98 - modResult);
        string checkDigit = checkDigitValue.ToString();

        return checkDigitValue > 9 ? checkDigit : "0" + checkDigit;
    }

    /// <summary>
    /// Calculates IBAN's check digit.
    /// ISO13616
    /// </summary>
    /// <param name="iban">IBAN object</param>
    /// <returns>Check digit as string</returns>
    /// <exception cref="InvalidIbanFormat">Thrown if supplied iban string contains invalid chracters</exception>
    public static string CalculateCheckDigit(Iban iban) => CalculateCheckDigit(iban.ToString());

    /// <summary>
    /// Returns IBAN's check digit
    /// </summary>
    /// <param name="iban">IBAN string value</param>
    /// <returns>Check digit string</returns>
    public static string GetCheckDigit(string iban) => iban.Substring(Check_Digit_Index, Check_Digit_Length);


    /// <summary>
    /// Returns IBAN's country code
    /// </summary>
    /// <param name="iban">IBAN string value</param>
    /// <returns>IBAN's country code string</returns>
    public static string GetCountryCode(string iban) => iban.Substring(Country_Code_Index, Country_Code_Length);

    /// <summary>
    /// Returns IBAN'S country code and check digit
    /// </summary>
    /// <param name="iban">IBAN string vlaue</param>
    /// <returns>IBAN's country code and check digit string</returns>
    public static string GetCountryCodeAndCheckDigit(string iban) => iban.Substring(Country_Code_Index, Country_Code_Length + Check_Digit_Length);

    /// <summary>
    /// Returns IBAN's BBAN code
    /// (all what is left without country code and check digit).
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <returns>BBAN string</returns>
    public static string GetBBan(string iban) => iban.Substring(Bban_Index);

    /// <summary>
    /// Returns IBAN's account number
    /// </summary>
    /// <param name="iban">IBAN string value</param>
    /// <returns>IBAN's account number as string</returns>
    public static string GetAccountNumber(string iban) => ExtractBbanEntry(iban, BBanEntryType.ACCOUNT_NUMBER);


    /// <summary>
    /// Returns IBAN's account number prefix
    /// </summary>
    /// <param name="iban">IBAN string value</param>
    /// <returns>IBAN's account number as string (if it is present)</returns>
    public static string GetAccountNumberPrefix(string iban) => ExtractBbanEntry(iban, BBanEntryType.ACCOUNT_NUMBER_PREFIX);

    /// <summary>
    /// Returns a new IBAN string with changed account number
    /// Automatically adds zeros to the beginning in order to maintain the length specified by the BBAN rule
    /// </summary>
    /// <param name="iban">Original IBAN</param>
    /// <param name="newAccountNumber">The new account number</param>
    /// <returns>IBAN with changed account number and recalculated check digit</returns>
    /// <exception cref="InvalidIbanFormat">Thrown when new account number is longer, than that is specified in BBAN rules</exception>
    public static string ChangeAccountNumber(string iban, string newAccountNumber) => ChangeBbanEntry(iban, newAccountNumber, BBanEntryType.ACCOUNT_NUMBER);

    /// <summary>
    /// Returns a new IBAN string with changed account number prefix
    /// Automatically adds zeros to the beginning in order to maintain the length specified by the BBAN rule
    /// </summary>
    /// <param name="iban">Original IBAN</param>
    /// <param name="newAccountNumberPrefix">The new account number prefix</param>
    /// <returns>IBAN with changed account number prefix and recalculated check digit</returns>
    /// <exception cref="InvalidIbanFormat">Thrown when new account number is longer, than that is specified in BBAN rules</exception>
    public static string ChangeAccountNumberPrefix(string iban, string newAccountNumberPrefix) => ChangeBbanEntry(iban, newAccountNumberPrefix, BBanEntryType.ACCOUNT_NUMBER_PREFIX);

    /// <summary>
    /// Returns IBAN'S bank code
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <returns>IBAN's bank code</returns>
    public static string GetBankCode(string iban) => ExtractBbanEntry(iban, BBanEntryType.BANK_CODE);

    /// <summary>
    /// Return a new IBAN string with changed bank code
    /// Automatically adds zeros to the beginning in order to maintain the length specified by the BBAN rule
    /// </summary>
    /// <param name="iban">Original IBAN</param>
    /// <param name="newBankCode">The new bank code</param>
    /// <returns>IBAN with changed bank code and recalculated check digit</returns>
    /// <exception cref="InvalidIbanFormat">Thrown when new bank code is longer, than that is specified in BBAN rules</exception>
    public static string ChangeBankCode(string iban, string newBankCode) => ChangeBbanEntry(iban, newBankCode, BBanEntryType.BANK_CODE);

    /// <summary>
    /// Returns IBAN's branch code
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <returns>IBAN's branch code string</returns>
    public static string GetBranchCode(string iban) => ExtractBbanEntry(iban, BBanEntryType.BRANCH_CODE);

    /// <summary>
    /// Returns IBAN's national check digit
    /// </summary>
    /// <param name="iban">Iban value string</param>
    /// <returns>IBAN's national check digit string</returns>
    public static string GetNationalCheckDigit(string iban) => ExtractBbanEntry(iban, BBanEntryType.NATIONAL_CHECK_DIGIT);

    /// <summary>
    /// Returns IBAN's account type
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <returns>IBAN's account type string</returns>
    public static string GetAccountType(string iban) => ExtractBbanEntry(iban, BBanEntryType.ACCOUNT_TYPE);

    /// <summary>
    /// Returns IBAN'S owner account type
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <returns>IBAN's owner account type string</returns>
    public static string GetOwnerAccountType(string iban) => ExtractBbanEntry(iban, BBanEntryType.OWNER_ACCOUNT_NUMBER);

    /// <summary>
    /// Returns IBAN's identification number
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <returns>IBAN's identifcation number string</returns>
    public static string GetIdentificationNumber(string iban) => ExtractBbanEntry(iban, BBanEntryType.IDENTIFICATION_NUMBER);



    /// <summary>
    /// Returns an iban with replaced check digit
    /// </summary>
    /// <param name="iban">Iban string value</param>
    /// <param name="checkDigit">A check digit which will be placed to IBAN string</param>
    /// <returns>IBAN string with replaced check digit</returns>
    public static string ReplaceCheckDigit(string iban, string checkDigit) => GetCountryCode(iban) + checkDigit + GetBBan(iban);



    private static void ValidateCheckDigit(string iban)
    {
        if (CalculateMod(iban) != 1)
        {
            string checkDigit = GetCheckDigit(iban);
            string expectedCheckDigit = CalculateCheckDigit(iban);

            throw new InvalidCheckDigit($"{iban} has invalid check digit {checkDigit}. Expected check digit is {expectedCheckDigit}", expectedCheckDigit, checkDigit);
        }
    }

    private static bool HasValidCheckDigitValue(string iban, out IbanFormatViolation validationResult)
    {
        validationResult = IbanFormatViolation.NO_VIOLATION;

        if (CalculateMod(iban) != 1)
        {
            validationResult = IbanFormatViolation.IBAN_INVALID_CHECK_DIGIT_VALUE;
        }

        return (validationResult == IbanFormatViolation.NO_VIOLATION);
    }

    private static void ValidateEmpty(string iban)
    {
        if (string.IsNullOrEmpty(iban))
        {
            throw new InvalidIbanFormat("Empty or null input cannot be a valid IBAN", IbanFormatViolation.IBAN_NOT_EMPTY_OR_NULL);
        }
    }

    private static void ValidateCountryCode(string iban)
    {
        if (iban.Length < Country_Code_Length)
        {
            throw new InvalidIbanFormat("Input must contain 2 letters for country code", IbanFormatViolation.COUNTRY_CODE_TWO_LETTERS, iban);
        }

        var countryCode = GetCountryCode(iban);

        if (!countryCode.Equals(countryCode.ToUpper()) || !char.IsLetter(iban[0]) || !char.IsLetter(iban[1]))
        {
            throw new InvalidIbanFormat("IBAN's country code must contain upper case letters", IbanFormatViolation.COUNTRY_CODE_UPPER_CASE_LETTERS, iban);
        }

        var countryEntry = CountryCode.GetCountryCode(countryCode);

        if (countryEntry == null)
        {
            throw new InvalidIbanFormat("IBAN contains non existing country code", IbanFormatViolation.COUNTRY_CODE_EXISTS, iban);
        }

        var structure = Bban.GetStructureForCountry(countryEntry);
        if (structure == null)
        {
            throw new UnsupportedCountry("IBAN contains not supported country code", countryCode);
        }
    }

    private static bool HasValidCountryCode(string iban, out IbanFormatViolation validationResult)
    {
        validationResult = IbanFormatViolation.NO_VIOLATION;

        if (iban.Length < Country_Code_Length)
        {
            validationResult = IbanFormatViolation.COUNTRY_CODE_TWO_LETTERS;
        }
        else
        {
            var countryCode = GetCountryCode(iban);
            if (!countryCode.Equals(countryCode.ToUpper()) || !char.IsLetter(iban[0]) || !char.IsLetter(iban[1]))
            {
                validationResult = IbanFormatViolation.COUNTRY_CODE_UPPER_CASE_LETTERS;
            }
            else
            {
                var countryEntry = CountryCode.GetCountryCode(countryCode);
                if (countryEntry == null)
                {
                    validationResult = IbanFormatViolation.COUNTRY_CODE_EXISTS;
                }
                else
                {
                    var structure = Bban.GetStructureForCountry(countryEntry);
                    if (structure == null)
                    {
                        validationResult = IbanFormatViolation.COUNTRY_CODE_UNSUPPORTED;
                    }
                }
            }
        }

        return (validationResult == IbanFormatViolation.NO_VIOLATION);
    }

    private static void ValidateCheckDigitPresence(string iban)
    {
        if (iban.Length < (Country_Code_Length + Check_Digit_Length))
        {
            throw new InvalidIbanFormat("IBAN must contain 2 digit check digit", IbanFormatViolation.CHECK_DIGIT_TWO_DIGITS, iban.Substring(Country_Code_Length));
        }

        string checkDigit = GetCheckDigit(iban);
        if (!char.IsDigit(checkDigit[0]) || !char.IsDigit(checkDigit[1]))
        {
            throw new InvalidIbanFormat("IBAN's check digit should contain only digits", IbanFormatViolation.CHECK_DIGIT_ONLY_DIGITS, checkDigit);
        }
    }

    private static bool HasValidCheckDigit(string iban, out IbanFormatViolation validationResult)
    {
        validationResult = IbanFormatViolation.NO_VIOLATION;

        if ((iban.Length < (Country_Code_Length + Check_Digit_Length)))
        {
            validationResult = IbanFormatViolation.CHECK_DIGIT_TWO_DIGITS;
        }
        else
        {
            string checkDigit = GetCheckDigit(iban);
            if (!char.IsDigit(checkDigit[0]) || !char.IsDigit(checkDigit[1]))
            {
                validationResult = IbanFormatViolation.CHECK_DIGIT_ONLY_DIGITS;
            }
        }

        return (validationResult == IbanFormatViolation.NO_VIOLATION);
    }

    private static void ValidateBbanLength(string iban, BBanStructure? structure)
    {
        var expectedBbanLength = structure?.GetBBanLength() ?? 0;
        var bban = GetBBan(iban);
        var bbanLength = bban.Length;

        if (expectedBbanLength != bbanLength)
        {
            throw new InvalidIbanFormat($"BBAN '{bban}' length is {bbanLength}, expected is {expectedBbanLength}",
                IbanFormatViolation.BBAN_LENGTH, bbanLength, expectedBbanLength);
        }
    }

    private static bool HasValidBbanLength(string iban, BBanStructure? structure, out IbanFormatViolation validationResult)
    {
        validationResult = IbanFormatViolation.NO_VIOLATION;

        var expectedBbanLength = structure?.GetBBanLength() ?? 0;
        var bban = GetBBan(iban);
        var bbanLength = bban.Length;

        if (expectedBbanLength != bbanLength)
        {
            validationResult = IbanFormatViolation.BBAN_LENGTH;
        }

        return (validationResult == IbanFormatViolation.NO_VIOLATION);
    }

    private static void ValidateBbanEntries(string iban, BBanStructure? structure)
    {
        var bban = GetBBan(iban);
        var bbanOffset = 0;

        foreach (var entry in structure?.Entries ?? new List<BBanEntry>())
        {
            var entryLength = entry.Length;
            var entryValue = bban.Substring(bbanOffset, entryLength);

            bbanOffset += entryLength;

            ValidateBbanEntryCharacterType(entry, entryValue);
        }
    }

    private static bool HasValidBbanEntries(string iban, BBanStructure? structure, out IbanFormatViolation validationResult)
    {
        validationResult = IbanFormatViolation.NO_VIOLATION;

        var bban = GetBBan(iban);
        var bbanOffset = 0;

        foreach (var entry in structure?.Entries ?? new List<BBanEntry>())
        {
            var entryLength = entry.Length;
            var entryValue = bban.Substring(bbanOffset, entryLength);

            bbanOffset += entryLength;

            if (!HasValidBbanEntryCharacterType(entry, entryValue, out validationResult))
            {
                break;
            }
        }

        return (validationResult == IbanFormatViolation.NO_VIOLATION);
    }

    private static void ValidateBbanEntryCharacterType(BBanEntry entry, string entryValue)
    {
        switch (entry.CharacterType)
        {
            case BBanEntryCharacterType.A:
                foreach (var c in entryValue)
                {
                    if (!char.IsUpper(c))
                    {
                        throw new InvalidIbanFormat($"'{entryValue}' must contain only upper case letters",
                            IbanFormatViolation.BBAN_ONLY_UPPER_CASE_LETTERS, c, entry.EntryType, entryValue);
                    }
                }
                break;
            case BBanEntryCharacterType.C:
                foreach (var c in entryValue)
                {
                    if (!char.IsLetterOrDigit(c))
                    {
                        throw new InvalidIbanFormat($"'{entryValue}' must contain only letters or digits",
                            IbanFormatViolation.BBAN_ONLY_DIGITS_OR_LETTERS, c, entry.EntryType, entryValue);
                    }
                }
                break;
            case BBanEntryCharacterType.N:
                foreach (var c in entryValue)
                {
                    if (!char.IsDigit(c))
                    {
                        throw new InvalidIbanFormat($"'{entryValue}' must contain only digits",
                            IbanFormatViolation.BBAN_ONLY_DIGITS, c, entry.EntryType, entryValue);
                    }
                }
                break;
        }
    }

    private static bool HasValidBbanEntryCharacterType(BBanEntry entry, string entryValue, out IbanFormatViolation validationResult)
    {
        validationResult = IbanFormatViolation.NO_VIOLATION;

        switch (entry.CharacterType)
        {
            case BBanEntryCharacterType.A:
                foreach (var c in entryValue)
                {
                    if (!char.IsUpper(c))
                    {
                        validationResult = IbanFormatViolation.BBAN_ONLY_UPPER_CASE_LETTERS;
                        break;
                    }
                }
                break;
            case BBanEntryCharacterType.C:
                foreach (var c in entryValue)
                {
                    if (!char.IsLetterOrDigit(c))
                    {
                        validationResult = IbanFormatViolation.BBAN_ONLY_DIGITS_OR_LETTERS;
                        break;
                    }
                }
                break;
            case BBanEntryCharacterType.N:
                foreach (var c in entryValue)
                {
                    if (!char.IsDigit(c))
                    {
                        validationResult = IbanFormatViolation.BBAN_ONLY_DIGITS;
                        break;
                    }
                }
                break;
        }

        return (validationResult == IbanFormatViolation.NO_VIOLATION);
    }

    private static int CalculateMod(string iban)
    {
        string reformattedIban = GetBBan(iban) + GetCountryCodeAndCheckDigit(iban);
        double total = 0;

        // a little java's workaround ;)
        char[] letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        for (int i = 0; i < reformattedIban.Length; i++)
        {
            double numericValue = char.IsLetter(reformattedIban[i]) ? (10 + Array.IndexOf(letters, reformattedIban[i])) : char.GetNumericValue(reformattedIban[i]);
            if (numericValue < 0 || numericValue > 35)
            {
                throw new InvalidIbanFormat($"Invalid character on position {i} = {numericValue}", IbanFormatViolation.IBAN_VALID_CHARACTERS, reformattedIban[i]);
            }

            total = (numericValue > 9 ? total * 100 : total * 10) + numericValue;

            if (total > Max)
            {
                total = (total % Mod);
            }
        }

        return (int)(total % Mod);
    }

    private static BBanStructure? GetBbanStructure(string iban)
    {
        string countryCode = GetCountryCode(iban);
        return GetBbanStructure(CountryCode.GetCountryCode(countryCode));
    }

    private static BBanStructure? GetBbanStructure(CountryCodeEntry? countryCode) => Bban.GetStructureForCountry(countryCode);

    private static string ExtractBbanEntry(string iban, BBanEntryType entryType)
    {
        var result = "";

        var bban = GetBBan(iban);
        var structure = GetBbanStructure(iban);
        var bbanOffset = 0;

        foreach (var entry in structure?.Entries ?? new List<BBanEntry>())
        {
            var entryLength = entry.Length;
            var entryValue = bban.Substring(bbanOffset, entryLength);

            bbanOffset += entryLength;

            if (entry.EntryType == entryType)
            {
                result = entryValue;
                break;
            }
        }

        return result;
    }

    private static string ChangeBbanEntry(string iban, string newValue, BBanEntryType entryType)
    {

        var bban = GetBBan(iban);
        var newIban = GetCountryCode(iban) + Iban.DEFAULT_CHECK_DIGIT;

        var structure = GetBbanStructure(iban);
        var bbanOffset = 0;
        var sb = new StringBuilder(bban);

        foreach (var entry in structure?.Entries ?? new List<BBanEntry>())
        {
            if (entry.EntryType == entryType)
            {

                if (newValue.Length > entry.Length)
                {
                    throw new InvalidIbanFormat($"New value for {Enum.GetName(typeof(BBanEntryType), entry.EntryType)} is too long.", IbanFormatViolation.BBAN_ENTRY_TOO_LONG);
                }

                sb.Remove(bbanOffset, entry.Length);
                sb.Insert(bbanOffset, newValue.PadLeft(entry.Length, '0'));
                break;
            }

            bbanOffset += entry.Length;
        }

        sb.Insert(0, newIban);
        newIban = sb.ToString();

        string newCheckDigit = CalculateCheckDigit(newIban);
        string result = ReplaceCheckDigit(newIban, newCheckDigit);

        return result;

    }
}