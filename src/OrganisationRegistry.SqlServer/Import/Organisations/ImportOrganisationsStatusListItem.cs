namespace OrganisationRegistry.SqlServer.Import.Organisations;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;

public class ImportOrganisationsStatusListItem
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string UserFirstName { get; set; } = null!;
    public string UserRoles { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FileContent { get; set; } = null!;
    public string? OutputFileContent { get; set; }
    public string Status { get; set; } = null!;
    public DateTimeOffset UploadedAt { get; set; }
    public DateTimeOffset? LastProcessedAt { get; set; }
    public string? ProcessInfo { get; set; }
    public string ImportFileType { get; set; } = null!;

    public static ImportOrganisationsStatusListItem Create(DateTimeOffset timestamp, IUser user, string fileName, string fileContent, string importFileType)
        => new()
        {
            UserId = user.UserId,
            UserName = user.LastName,
            UserFirstName = user.FirstName,
            UserRoles = string.Join("|", user.Roles),
            FileName = fileName,
            FileContent = fileContent,
            ImportFileType = importFileType,
            Id = Guid.NewGuid(),
            Status = ImportProcessStatus.Processing,
            UploadedAt = timestamp,
        };
}

public static class ImportProcessStatus
{
    public const string Processing = "Aan het verwerken";
    public const string Processed = "Geslaagd";
    public const string Failed = "Gefaald";
}

public class ImportStatusListItemConfiguration : EntityMappingConfiguration<ImportOrganisationsStatusListItem>
{
    public override void Map(EntityTypeBuilder<ImportOrganisationsStatusListItem> b)
    {
        b.ToTable("ImportOrganisationsStatusList", WellknownSchemas.ImportSchema)
            .HasKey(p => p.Id);

        b.Property(p => p.Id);
        b.Property(p => p.UserId);
        b.Property(p => p.UserName);
        b.Property(p => p.UserFirstName);
        b.Property(p => p.UserRoles);
        b.Property(p => p.FileName);
        b.Property(p => p.FileContent).HasMaxLength(int.MaxValue);
        b.Property(p => p.OutputFileContent).HasMaxLength(int.MaxValue);
        b.Property(p => p.Status);
        b.Property(p => p.UploadedAt);
        b.Property(p => p.LastProcessedAt);
        b.Property(p => p.ProcessInfo).HasMaxLength(int.MaxValue);
        b.Property(p => p.ImportFileType);
    }
}
