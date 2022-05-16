namespace OrganisationRegistry.SqlServer.Import.Organisations;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

public class ImportOrganisationsStatusListItem
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string FileName { get; set; }
    public string FileContent { get; set; }
    public string Status { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
    public DateTimeOffset? LastProcessedAt { get; set; }
    public string? ProcessInfo { get; set; }
}

public static class ImportProcessStatus
{
    public const string Processing = "Processing";
    public const string Processed = "Processed";
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
        b.Property(p => p.FileName);
        b.Property(p => p.FileContent).HasMaxLength(int.MaxValue);
        b.Property(p => p.Status);
        b.Property(p => p.UploadedAt);
        b.Property(p => p.LastProcessedAt);
        b.Property(p => p.ProcessInfo).HasMaxLength(int.MaxValue);
    }
}
