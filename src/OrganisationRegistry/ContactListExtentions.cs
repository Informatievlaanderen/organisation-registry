namespace OrganisationRegistry;

using System.Collections.Generic;
using System.Linq;
using Organisation;

public static class ContactListExtentions
{
    public static void ThrowIfAnyInvalid(this IEnumerable<Contact> contacts)
        => contacts
            .Where(contact => !string.IsNullOrEmpty(contact.Value))
            .ToList()
            .ForEach(contact => contact.ContactType.ThrowIfInvalidValue(contact.Value));
}
