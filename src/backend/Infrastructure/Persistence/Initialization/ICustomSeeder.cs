namespace CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;
public interface ICustomSeeder
{
    Task InitializeAsync(CancellationToken cancellationToken);
}