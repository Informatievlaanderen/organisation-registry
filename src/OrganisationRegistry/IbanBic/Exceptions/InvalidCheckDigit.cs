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
/// Exception which is thrown to indicate that IBAN's check digit is invalid.
/// </summary>
public class InvalidCheckDigit : Exception
{
    public string? ActualString { get; }
    public string? ExpectedString { get; }

    public InvalidCheckDigit()
    { }

    public InvalidCheckDigit(string message) : base(message)
    { }

    public InvalidCheckDigit(string message, Exception innerException) : base(message, innerException)
    { }

    public InvalidCheckDigit(string format, params object[] args) : base(string.Format(format, args))
    { }

    public InvalidCheckDigit(string message, string expected, string actual) : base(message)
    {
        ActualString = actual;
        ExpectedString = expected;
    }
}
