using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CodeMatrix.Mepd.Infrastructure.Seeding;

//public class AdminLevelSeeder : ICustomSeeder
//{
//    private readonly ISerializerService _serializerService;
//    private readonly ApplicationDbContext _db;
//    private readonly ILogger<AdminLevelSeeder> _logger;

//    public AdminLevelSeeder(ISerializerService serializerService, ILogger<AdminLevelSeeder> logger, ApplicationDbContext db)
//    {
//        _serializerService = serializerService;
//        _logger = logger;
//        _db = db;
//    }

//    public async Task InitializeAsync(CancellationToken cancellationToken)
//    {
//        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

//        if (!_db.AdminLevels.Any())
//        {
//            _logger.LogInformation("Started to Seed Admin Levels.");

//            // Here you can use your own logic to populate the database.
//            // As an example, I am using a JSON file to populate the database.
//            string data = await File.ReadAllTextAsync(path + "/Seeding/adminLevels.json", cancellationToken);
//            var records = _serializerService.Deserialize<List<AdminLevel>>(data);

//            if (records != null)
//            {
//                foreach (var record in records)
//                {
//                    await _db.AdminLevels.AddAsync(record, cancellationToken);
//                }
//            }

//            await _db.SaveChangesAsync(cancellationToken);
//            _logger.LogInformation("Seeded Admin Levels.");
//        }
//    }
//}