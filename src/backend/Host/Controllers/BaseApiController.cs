using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeMatrix.Mepd.Host.Controllers;

/// <summary>
/// Api base controller
/// </summary>
[ApiController]
[ApiConventionType(typeof(MepdApiConventions))]
public class BaseApiController : ControllerBase
{
    private ISender _mediator = null;

    /// <summary>
    /// Mediator instance
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
}