namespace OrganisationRegistry.Configuration.Database;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public static class EntityFrameworkExtensions
{
    public static IConfigurationBuilder AddEntityFramework(
        this IConfigurationBuilder builder,
        Action<DbContextOptionsBuilder> options)
    {
        return builder.Add(new EntityFrameworkConfigurationSource(options));
    }
}