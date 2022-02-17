using CodeMatrix.Mepd.Application.Common.Events;
using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Domain.Dump;
using CodeMatrix.Mepd.Infrastructure.Persistence.Configurations;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Options;


namespace CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : BaseDbContext
{
    public DbSet<StudyMajor> StudyMajors=> Set<StudyMajor>();

    public ApplicationDbContext(
        ITenantInfo currentTenant,
        DbContextOptions options,
        ICurrentUser currentUser, 
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventService eventService,
        IEncryptionProvider encryptionProvider)
    : base(currentTenant, options, currentUser, serializer, dbSettings, eventService, encryptionProvider)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Default);
    }
}