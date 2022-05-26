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

namespace OrganisationRegistry.IbanBic
{
    using Exceptions;

    /// <summary>
    /// Business Identifier Codes (also known as SWIFT-BIC, BIC code, SWIFT ID or SWIFT code).
    /// ISO 9362
    /// </summary>
    public class Bic
    {
        public string Value { get; }

        private Bic(string value)
        {
            Value = value;
        }

        /// <summary>
        /// BIC object creation
        /// </summary>
        /// <param name="bicCode">The string to be parsed as BIC code.</param>
        /// <returns>BIC object holding the value represented by the string argument.</returns>
        /// <exception cref="InvalidBicFormat">If the string contains invalid BIC code<./exception>
        /// <exception cref="UnsupportedCountry">If BIC's country is not supported.</exception>
        public static Bic CreateInstance(string bicCode)
        {
            BicUtils.ValidateBIC(bicCode);
            return new Bic(bicCode);
        }

        /// <summary>
        /// Bank code from the BIC
        /// </summary>
        public string BankCode => BicUtils.GetBankCode(Value);

        /// <summary>
        /// Country code from the BIC
        /// </summary>
        /// <returns>CountryCodeEntry representation of BIC's country code</returns>
        public CountryCodeEntry GetCountryCode() => CountryCode.GetCountryCode(BicUtils.GetCountryCode(Value));

        /// <summary>
        /// Location code from the BIC
        /// </summary>
        public string LocationCode => BicUtils.GetLocationCode(Value);

        /// <summary>
        /// Branch code from the BIC
        /// </summary>
        /// <returns>String represenation of the BIC's branch code, empty string if BIC has no branch code.</returns>
        public string GetBranchCode()
            => BicUtils.HasBranchCode(Value) ? BicUtils.GetBranchCode(Value) : "";

        public override int GetHashCode()
            => Value.GetHashCode();

        public override string ToString()
            => Value;

        public override bool Equals(object? obj)
            => obj is Bic bic && Value.Equals(bic.Value);
    }
}
