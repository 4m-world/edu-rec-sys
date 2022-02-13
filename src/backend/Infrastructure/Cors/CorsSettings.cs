namespace CodeMatrix.Mepd.Infrastructure.Cors;

public class CorsSettings
{
    public string Url { get; set; }
    public List<CorsClients> Clients { get; set; }
}

public class CorsClients
{
    public string Policy { get; set; }
    public string Url { get; set; }
}