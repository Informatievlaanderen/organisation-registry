namespace OrganisationRegistry.SqlServer.Log
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;

    public class Log
    {
        public Guid Id { get; set; }
        public string Application { get; set; }
        public DateTime Logged { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
    }

    public class KeyListConfiguration : EntityMappingConfiguration<Log>
    {
        public override void Map(EntityTypeBuilder<Log> b)
        {
            b.ToTable("Logs", "OrganisationRegistry")
                .HasKey(p => p.Id);

            b.Property(p => p.Application);
            b.Property(p => p.Logged);
            b.Property(p => p.Message);
            b.Property(p => p.Logger);
            b.Property(p => p.Exception);
            b.Property(p => p.Level).HasMaxLength(50);
            b.Property(p => p.Logger).HasMaxLength(250);
        }
    }
}
