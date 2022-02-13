namespace CodeMatrix.Mepd.Application.Common.Persistence;

/// <summary>
/// Anonymize connection string contract to hide sensitve information
/// </summary>
public interface IAnonymizeConnectionString
{
    /// <summary>
    /// Anonymize connection string by hidding sensitve inforamtion
    /// </summary>
    /// <param name="connectionString">connection string</param>
    /// <param name="dbProvider">Database provider</param>
    /// <returns>Anonymized connection string</returns>
    string Anonymize(string connectionString, string dbProvider = default);
}
