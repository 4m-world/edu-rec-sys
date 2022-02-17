namespace CodeMatrix.Mepd.Application.Dump;

public class PredictStudyMajorRequest : IRequest<PredictedResponse>
{
    public Gender Gender { get; set; }
    public float Gpa { get; set; }
    public float MathScore { get; set; }
    public float PhysicsScore { get; set; }
    public bool Art { get; set; }
    public bool Analytic { get; set; }
    public bool Assembly { get; set; }
    public bool Electronic { get; set; }
    public bool Mechanical { get; set; }
    public bool Offsite { get; set; }
}
