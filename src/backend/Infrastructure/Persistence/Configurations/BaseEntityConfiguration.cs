// -----------------------------------------------------------------------------
// CodeMatrix.MagicCode 
// Copyright © 2021 Magic Code, Code Matrix
// Version：1.0.0
// -----------------------------------------------------------------------------

using CodeMatrix.Mepd.Domain.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeMatrix.Mepd.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Base Entity Configuration
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : AuditableEntity
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedBy);
            builder.Property(e => e.CreatedOn)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.LastModifiedBy);
            builder.Property(e => e.LastModifiedOn);

            ConfigureEntity(builder);
        }

        /// <summary>
        /// Configure entiry
        /// </summary>
        /// <param name="builder"></param>
        public abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
    }
}
