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

namespace OrganisationRegistry.IbanBic.Exceptions
{
    using System;

    /// <summary>
    /// Exception which is thrown, when the application attempts to convert a string to IBAN,
    /// but that the string does not have the appropriate format.
    /// Contains a field for indication which rule of validation is broken an which character
    /// in the string causes failure.
    /// </summary>
    public class InvalidIbanFormat : Exception
    {
        public IbanFormatViolation FormatViolation { get; private set; }
        public object ExpectedObject { get; private set; }
        public object ActualObject { get; private set; }
        public BBanEntryType BBanEntryType { get; private set; }
        public char InvalidCharacter { get; private set; }

        public InvalidIbanFormat() : base()
        { }

        public InvalidIbanFormat(string message) : base(message)
        { }

        public InvalidIbanFormat(string message, Exception innerException) : base(message, innerException)
        { }

        public InvalidIbanFormat(string format, params object[] args) : base(string.Format(format, args))
        { }

        public InvalidIbanFormat(string message, IbanFormatViolation formatViolation, Exception innerException) : base(message, innerException)
        {
            FormatViolation = formatViolation;
        }

        public InvalidIbanFormat(string message, IbanFormatViolation formatViolation) : base(message)
        {
            FormatViolation = formatViolation;
        }

        public InvalidIbanFormat(string message, IbanFormatViolation formatViolation, object actual, object expected) : base(message)
        {
            FormatViolation = formatViolation;
            ActualObject = actual;
            ExpectedObject = expected;
        }

        public InvalidIbanFormat(string message, IbanFormatViolation formatViolation, object actual) : base(message)
        {
            FormatViolation = formatViolation;
            ActualObject = actual;
        }

        public InvalidIbanFormat(string message, IbanFormatViolation formatViolation, char invalidCharacter) : base(message)
        {
            FormatViolation = formatViolation;
            InvalidCharacter = invalidCharacter;
        }

        public InvalidIbanFormat(string message, IbanFormatViolation formatViolation, char invalidCharacter, BBanEntryType entryType, object actual) : base(message)
        {
            FormatViolation = formatViolation;
            InvalidCharacter = invalidCharacter;
            BBanEntryType = entryType;
            ActualObject = actual;
        }
    }
}
