using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using static Lorincz_Denisa_Lab4.PricePredictionModel;
public class BatchPredictionController : Controller
{
    private readonly IWebHostEnvironment _env;

    public BatchPredictionController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet]
    public IActionResult Batch()
    {
        return View(new BatchPredictionViewModel());
    }

    [HttpPost]
    public IActionResult Batch(BatchPredictionViewModel model)
    {
        if (model.File == null || model.File.Length == 0)
        {
            model.ErrorMessage = "Vă rugăm să selectați un fișier CSV.";
            return View(model);
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var tempFilePath = Path.Combine(uploadsFolder, Guid.NewGuid() + ".csv");
        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            model.File.CopyTo(stream);
        }

        var mlContext = new MLContext();
        ITransformer mlModel = mlContext.Model.Load("PricePredictionModel.mlnet", out _);

        var dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
            path: tempFilePath,
            hasHeader: true,
            separatorChar: ',');

        var predictionsDataView = mlModel.Transform(dataView);

        var predictions = mlContext.Data.CreateEnumerable<ModelOutput>(
            predictionsDataView,
            reuseRowObject: false).ToList();

        System.IO.File.Delete(tempFilePath);

        model.Predictions = predictions;
        return View(model);
    }
}
