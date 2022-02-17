using CodeMatrix.Mepd.Application.Dump.Models;
using CodeMatrix.Mepd.Domain.Dump;
using Microsoft.Extensions.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;

namespace CodeMatrix.Mepd.Application.Dump;


internal class PredictStudyMajorRequestHandler : IRequestHandler<PredictStudyMajorRequest, PredictedResponse>
{
    private readonly ILogger<PredictStudyMajorRequestHandler> _logger;
    private readonly IStringLocalizer<PredictStudyMajorRequestHandler> _localizer;
    private readonly IReadRepository<StudyMajor> _studyMajorRepository;
    private readonly PredictionEnginePool<StudentProfileModel, StudentMajorPredictOutput> _enginePool;

    public PredictStudyMajorRequestHandler(
        PredictionEnginePool<StudentProfileModel, StudentMajorPredictOutput> enginePool,
        ILogger<PredictStudyMajorRequestHandler> logger,
        IStringLocalizer<PredictStudyMajorRequestHandler> localizer,
        IReadRepository<StudyMajor> studyMajorRepository)
    {
        _logger = logger;
        _localizer = localizer;
        _studyMajorRepository = studyMajorRepository;
        _enginePool = enginePool;
    }

    public async Task<PredictedResponse> Handle(PredictStudyMajorRequest request, CancellationToken cancellationToken)
    {
        /**
         * 1. Convert request into model input
         * 2. Predeict the model output
         * 3. Get top three results from the model output
         * 4. Get Study Major details of the thre majors
         * 5. Check if the suggested majors score is less than 50% then :'-( 
         */
        _logger.LogInformation(_localizer["Started Predicting study major for"]);
        _logger.LogInformation(JsonConvert.SerializeObject(request));
        var model = new StudentProfileModel
        {
            Sex = (int)request.Gender,
            Gpa = request.Gpa,
            Math = request.MathScore,
            Physics = request.PhysicsScore,
            Analytic = request.Analytic ? 1 : 0,
            Art = request.Art ? 1 : 0,
            Construct = request.Assembly ? 1 : 0,
            Electric = request.Electronic ? 1 : 0,
            Mechanic = request.Mechanical ? 1 : 0,
        };

        var output = _enginePool.Predict(modelName: Dump.DumpConstants.ModelName, example: model);

        var topNScores = GetTopNRanks(DumpConstants.ModelName, output);
        _logger.LogInformation("[" + JsonConvert.SerializeObject(topNScores) + "]");


        var codes = topNScores.Select(e => e.Key).ToArray();

        var majors = await _studyMajorRepository.ListAsync(new GetStudyMajorsByCodesSpec(topNScores.Select(e => e.Key).ToArray()), cancellationToken);

        var topScore = GetPredictedScore(DumpConstants.ModelName, output);


        var response = new PredictedResponse()
        {
            SuggestedMajor = majors.Select(e => new SuggestedMajor
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Score = topNScores.Single(n => n.Key.Trim() == e.Code.Trim()).Value
            }).ToList(),
            Message = _localizer["Based on the data provided, these specializations are suitable for you from the top rank to the lowest."]
            //Message = topScore < .50
            //? _localizer["The provided data are not sufficient to give an accurate recommendation, as the predicted score is less than 50%."]
            //: _localizer["Based on the data provided, these specializations are suitable for you from the top rank to the lowest."]
        };
        _logger.LogInformation("[" + JsonConvert.SerializeObject(topNScores) + "]");
        _logger.LogInformation(JsonConvert.SerializeObject(output));
        _logger.LogInformation(_localizer["Complete Predicting study major"]);

        return response;
    }

    private IEnumerable<KeyValuePair<string, decimal>> GetTopNRanks(string modelName, StudentMajorPredictOutput output, int n = 3)
    {
        if (n < 0) throw new ArgumentException(string.Format(_localizer["Invlaid number of required ranks '{0}' value"], n), nameof(n));

        var labelBuffer = new VBuffer<ReadOnlyMemory<char>>();
        _enginePool.GetPredictionEngine(modelName: Dump.DumpConstants.ModelName).OutputSchema["Score"].Annotations.GetValue("SlotNames", ref labelBuffer);
        var labels = labelBuffer.DenseValues().Select(l => l.ToString()).ToArray();

        var topNScores = labels.ToDictionary(
           l => l,
           l => (decimal)output.Score[Array.IndexOf(labels, l)]
           )
           .OrderByDescending(kv => kv.Value)
           .Take(n);

        return topNScores;
    }

    private float GetPredictedScore(string modelName, StudentMajorPredictOutput output)
    {
        var labelBuffer = new VBuffer<ReadOnlyMemory<char>>();
        _enginePool.GetPredictionEngine(modelName: Dump.DumpConstants.ModelName).OutputSchema["Score"].Annotations.GetValue("SlotNames", ref labelBuffer);
        var labels = labelBuffer.DenseValues().Select(l => l.ToString()).ToArray();

        var index = Array.IndexOf(labels, output.PredictedLabel);
        var score = output.Score[index];

        return score;
    }
}
