using Microsoft.AspNetCore.Mvc;

namespace CodeMatrix.Mepd.Host.Controllers;

/// <summary>
/// Versioned api controller
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
public class VersionedApiController : BaseApiController
{
}