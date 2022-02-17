using CodeMatrix.Mepd.Application.Dump;

namespace CodeMatrix.Mepd.Host.Controllers
{
    [AllowAnonymous]
    public class RecommendController : VersionedApiController
    {
        [HttpPost]
        [TenantIdHeader]
        public Task<PredictedResponse> PredictStudyFieldAsync(PredictStudyMajorRequest request)
        {
            return Mediator.Send(request);
        }
    }
}
