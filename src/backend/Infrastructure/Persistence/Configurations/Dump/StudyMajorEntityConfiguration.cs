using CodeMatrix.Mepd.Domain.Dump;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeMatrix.Mepd.Infrastructure.Persistence.Configurations.Dump;

public class StudyMajorEntityConfiguration : BaseEntityConfiguration<StudyMajor>
{
    public override void ConfigureEntity(EntityTypeBuilder<StudyMajor> builder)
    {
        builder.ToTable(nameof(StudyMajor), SchemaNames.Dump);

        builder.Property(e => e.Name).HasMaxLength(75).IsUnicode().IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500).IsUnicode();
        builder.Property(e => e.Code).HasMaxLength(10).IsRequired().IsRequired();

        builder.HasIndex(e => e.Code).IsUnique(false);
        builder.IsMultiTenant();
    }
}
