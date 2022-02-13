using Microsoft.AspNetCore.Mvc;

namespace CodeMatrix.Mepd.Host.Controllers;

/// <summary>
/// Version neutral api controller
/// </summary>
[Route("api/[controller]")]
[ApiVersionNeutral]
public class VersionNeutralApiController : BaseApiController
{
}
