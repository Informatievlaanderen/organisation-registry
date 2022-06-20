// https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonSerializerSettingsProvider.cs
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace OrganisationRegistry.Infrastructure.Infrastructure.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Helper class which provides <see cref="JsonSerializerSettings"/>.
/// </summary>
public static class JsonSerializerSettingsProvider
{
    private const int DefaultMaxDepth = 32;

    /// <summary>
    /// Creates default <see cref="JsonSerializerSettings"/>.
    /// </summary>
    /// <returns>Default <see cref="JsonSerializerSettings"/>.</returns>
    public static JsonSerializerSettings CreateSerializerSettings()
        => new()
        {
            ContractResolver = new OrganisationRegistryContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            },

            MissingMemberHandling = MissingMemberHandling.Ignore,

            // Limit the object graph we'll consume to a fixed depth. This prevents stackoverflow exceptions
            // from deserialization errors that might occur from deeply nested objects.
            MaxDepth = DefaultMaxDepth,

            // Do not change this setting
            // Setting this to None prevents Json.NET from loading malicious, unsafe, or security-sensitive types
            TypeNameHandling = TypeNameHandling.None,
        };
}
