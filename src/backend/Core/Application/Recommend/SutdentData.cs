using Microsoft.ML.Data;

namespace CodeMatrix.Mepd.Application.Recommend;

public class SutdentData
{
    [LoadColumn(1)]
    public float Gpa { get; set; }

    [LoadColumn(0)]
    public string Gender { get; set; }

    [LoadColumn(3)]
    public float Physics { get; set; }

    [LoadColumn(4)]
    public float Math { get; set; }

    [LoadColumn(5)]
    public float Art { get; set; }

    [LoadColumn(6)]
    public float Analytics { get; set; }

    [LoadColumn(7)]
    public float Construction { get; set; }

    [LoadColumn(8)]
    public float Electronics { get; set; }

    [LoadColumn(9)]
    public float Mechanical { get; set; }

    [LoadColumn(10)]
    public float Offsite { get; set; }
}

public class StudyPrediction
{
    [ColumnName("PredictionLabel")]
    public string Prediction { get; set; }
}