using CodeMatrix.Mepd.Application.Common.Persistence;
using CodeMatrix.Mepd.Infrastructure.Common;
using CodeMatrix.Mepd.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace CodeMatrix.Mepd.Infrastructure.Multitenancy;

/// <summary>
/// Anonymize connection string  to hide sensitve information
/// </summary>
public class AnonymizeConnectionString : IAnonymizeConnectionString
{
    private const string HiddenValueDefault = "*******";
    private readonly DatabaseSettings _dbSettings;

    public AnonymizeConnectionString(IOptions<DatabaseSettings> dbSettings) =>
        _dbSettings = dbSettings.Value;


    /// <inheritdoc />
    public string Anonymize(string connectionString, string dbProvider = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return connectionString;

        if (string.IsNullOrWhiteSpace(dbProvider))
        {
            dbProvider = _dbSettings.DBProvider;
        }

        return dbProvider.ToLowerInvariant() switch
        {
            DbProviderKeys.SqlServer => AnonyymizeSqlServerConnection(connectionString),
            _ => connectionString
        };
    }

    private static string AnonyymizeSqlServerConnection(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        if (!string.IsNullOrEmpty(builder.Password) || !builder.IntegratedSecurity)
        {
            builder.Password = HiddenValueDefault;
        }

        if (!string.IsNullOrEmpty(builder.UserID) || !builder.IntegratedSecurity)
        {
            builder.UserID = HiddenValueDefault;
        }

        return builder.ConnectionString;
    }
}
