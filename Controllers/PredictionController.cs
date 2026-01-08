using Lorincz_Denisa_Lab4;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Lorincz_Denisa_Lab4;
using Lorincz_Denisa_Lab4.Data;
using Lorincz_Denisa_Lab4.Models;
using System;

namespace Lorincz_Denisa_Lab4.Controllers
{
    public class PredictionController : Controller
    {

        private readonly AppDbContext _context;

        public PredictionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Price()
        {
            return View(new Lorincz_Denisa_Lab4.PricePredictionModel.ModelInput());
        }

        // --- Price Prediction ---
        [HttpPost]
        public async Task<IActionResult> Price(PricePredictionModel.ModelInput input)
        {
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load(@"PricePredictionModel.mlnet", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<PricePredictionModel.ModelInput, PricePredictionModel.ModelOutput>(mlModel);

            PricePredictionModel.ModelOutput result = predEngine.Predict(input);

            ViewBag.Price = result.Score;

            var history = new PredictionHistory
            {
                PassengerCount = input.Passenger_count,
                TripTimeInSecs = input.Trip_time_in_secs,
                TripDistance = input.Trip_distance,
                PaymentType = input.Payment_type,
                PredictedPrice = result.Score,
                CreatedAt = DateTime.Now
            };

            _context.PredictionHistories.Add(history);
            await _context.SaveChangesAsync();

            return View(input);

        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var history = await _context.PredictionHistories
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(history);
        }

        // --- Time Prediction ---
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