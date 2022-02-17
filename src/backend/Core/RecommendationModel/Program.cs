using CodeMatrix.Mepd.Application.Recommend;
using Microsoft.Extensions.Configuration;
using Microsoft.ML;

namespace CodeMatrix.Mepd.RecommendationModel;

class Program
{
    private static readonly string fileName = "study_model";
    
    static async Task Main(string[] args)
    {
        var path = Directory.GetCurrentDirectory();

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("config.json");

        var config = builder.Build();
        var trainingFile = config["training_data"];
        var testingFile = config["testing_data"];

        try
        {
            var trainingDataPath = Path.Combine(path, "Data", trainingFile);
            var testingDataPath = Path.Combine(path, "Data", testingFile);
            var context = new MLContext();

            Console.WriteLine($"=========== Start Loading Data ===========");
            var trainingDataView = context.Data.LoadFromTextFile<SutdentData>(trainingDataPath, hasHeader: true);
            Console.WriteLine($"=========== Complete Loading Data ===========");

            Console.WriteLine($"=============== Start Processing Data ===============");
            var pipline = context.Transforms.Conversion.MapValueToKey(inputColumnName: "Output", outputColumnName: "PredictionLabel")
                .Append(context.Transforms.Text.FeaturizeText(inputColumnName: "Gender"));

        }
        catch (Exception ex)
        {
            Console.Write("Error: ");
            Console.WriteLine(ex.Message);
        }
        
    }
}