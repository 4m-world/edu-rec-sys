namespace CodeMatrix.Mepd.Infrastructure.Persistence;

public class DatabaseSettings
{
    public string DBProvider { get; set; }
    public string ConnectionString { get; set; }

    public string EncryptionKey { get; set; }
    public string EncryptionIV { get; set; }
}