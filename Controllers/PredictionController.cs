using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace Lorincz_Denisa_Lab4.Controllers
{
    public class PredictionController : Controller
    {
        // --- Pricee Prediction ---
        public IActionResult Price(PricePredictionModel.ModelInput input)
        {
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load(@"PricePredictionModel.mlnet", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<PricePredictionModel.ModelInput, PricePredictionModel.ModelOutput>(mlModel);

            PricePredictionModel.ModelOutput result = predEngine.Predict(input);

            ViewBag.Price = result.Score;

            return View(input);
        }

         //--- Time Prediction ---
        public IActionResult Time(TimePredictionModel.ModelInput input)
        {
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load("TimePredictionModel.mlnet", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<TimePredictionModel.ModelInput, TimePredictionModel.ModelOutput>(mlModel);

            TimePredictionModel.ModelOutput result = predEngine.Predict(input);

            ViewBag.Time = result.Score;

            return View(input);
        }
    }
}