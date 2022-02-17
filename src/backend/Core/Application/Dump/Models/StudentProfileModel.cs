using Microsoft.ML.Data;

namespace CodeMatrix.Mepd.Application.Dump.Models;

public class StudentProfileModel
{
    [ColumnName(@"sex")]
    public float Sex { get; set; }

    [ColumnName(@"gpa")]
    public float Gpa { get; set; }

    [ColumnName(@"physics")]
    public float Physics { get; set; }

    [ColumnName(@"math")]
    public float Math { get; set; }

    [ColumnName(@"art")]
    public float Art { get; set; }

    [ColumnName(@"analytic")]
    public float Analytic { get; set; }

    [ColumnName(@"construct")]
    public float Construct { get; set; }

    [ColumnName(@"electric")]
    public float Electric { get; set; }

    [ColumnName(@"mechanic")]
    public float Mechanic { get; set; }

    [ColumnName(@"specialty")]
    public string Specialty { get; set; }

}

public class StudentMajorPredictOutput
{
    [ColumnName(@"sex")]
    public float Sex { get; set; }

    [ColumnName(@"gpa")]
    public float Gpa { get; set; }

    [ColumnName(@"physics")]
    public float Physics { get; set; }

    [ColumnName(@"math")]
    public float Math { get; set; }

    [ColumnName(@"art")]
    public float Art { get; set; }

    [ColumnName(@"analytic")]
    public float Analytic { get; set; }

    [ColumnName(@"construct")]
    public float Construct { get; set; }

    [ColumnName(@"electric")]
    public float Electric { get; set; }

    [ColumnName(@"mechanic")]
    public float Mechanic { get; set; }

    [ColumnName(@"specialty")]
    public uint Specialty { get; set; }

    [ColumnName(@"Features")]
    public float[] Features { get; set; }

    [ColumnName(@"PredictedLabel")]
    public string PredictedLabel { get; set; }

    [ColumnName(@"Score")]
    public float[] Score { get; set; }
}