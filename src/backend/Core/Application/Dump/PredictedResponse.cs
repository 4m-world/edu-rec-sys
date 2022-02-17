namespace CodeMatrix.Mepd.Application.Dump;

public class PredictedResponse
{
    public string Message { get; set; }

    public IList<SuggestedMajor> SuggestedMajor { get; set; }
}
