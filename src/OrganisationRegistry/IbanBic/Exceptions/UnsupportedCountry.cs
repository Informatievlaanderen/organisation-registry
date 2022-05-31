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

namespace OrganisationRegistry.IbanBic.Exceptions;

using System;

/// <summary>
/// Exception whis is thrown, when the requested country is not supported
/// </summary>
public class UnsupportedCountry : Exception
{
    public string? CountryCode { get; }

    public UnsupportedCountry()
    { }

    public UnsupportedCountry(string message) : base(message)
    { }

    public UnsupportedCountry(string message, string countryCode) : base(message)
    {
        CountryCode = countryCode;
    }

    public UnsupportedCountry(string message, Exception innerException) : base(message, innerException)
    { }

}
