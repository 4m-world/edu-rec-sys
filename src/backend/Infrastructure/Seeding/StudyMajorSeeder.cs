using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Domain.Dump;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CodeMatrix.Mepd.Infrastructure.Seeding;

public class StudyMajorSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<StudyMajorSeeder> _logger;

    public StudyMajorSeeder(ISerializerService serializerService, ILogger<StudyMajorSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!_db.StudyMajors.Any())
        {
            _logger.LogInformation("Started to Seed Study Majors.");

            string data = await File.ReadAllTextAsync(path + "/Seeding/dum_study_major.json", cancellationToken);
            var records = _serializerService.Deserialize<List<StudyMajor>>(data);

            if (records != null)
            {
                await _db.StudyMajors.AddRangeAsync(records, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Seeded Study Majors.");
        }
    }
}