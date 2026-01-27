using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lorincz_Denisa_Lab4.Data;

public class PredictionController : Controller
{
    private readonly AppDbContext _context;

    public PredictionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> History(
        string? paymentType,
        float? minPrice,
        float? maxPrice,
        string? sortOrder)
    {
        var query = _context.PredictionHistories.AsQueryable();

        // filtrare tip plata
        if (!string.IsNullOrEmpty(paymentType))
        {
            query = query.Where(p => p.PaymentType == paymentType);
        }

        // filtrare pret
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.PredictedPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.PredictedPrice <= maxPrice.Value);
        }

        // sortare pret (optional / conform punctului 3 din PDF)
        query = sortOrder switch
        {
            "price_asc" => query.OrderBy(p => p.PredictedPrice),
            "price_desc" => query.OrderByDescending(p => p.PredictedPrice),
            _ => query.OrderBy(p => p.PredictedPrice) // default ca in PDF
        };

        ViewBag.CurrentPaymentType = paymentType;
        ViewBag.CurrentMinPrice = minPrice;
        ViewBag.CurrentMaxPrice = maxPrice;
        ViewBag.CurrentSortOrder = sortOrder;

        var result = await query.ToListAsync();
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.PredictionHistories.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(p => p.CreatedAt.Date >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(p => p.CreatedAt.Date <= toDate.Value.Date);

        var totalPredictions = await query.CountAsync();

        var paymentTypeStats = await query
            .GroupBy(p => p.PaymentType)
            .Select(g => new PaymentTypeStat
            {
                PaymentType = g.Key,
                AveragePrice = g.Average(x => x.PredictedPrice),
                Count = g.Count()
            })
            .ToListAsync();

        var prices = await query.Select(p => p.PredictedPrice).ToListAsync();

        var buckets = new List<PriceBucketStat>
    {
        new PriceBucketStat { Label = "0 - 10" },
        new PriceBucketStat { Label = "10 - 20" },
        new PriceBucketStat { Label = "20 - 30" },
        new PriceBucketStat { Label = "30 - 50" },
        new PriceBucketStat { Label = "> 50" }
    };

        foreach (var price in prices)
        {
            if (price < 10) buckets[0].Count++;
            else if (price < 20) buckets[1].Count++;
            else if (price < 30) buckets[2].Count++;
            else if (price < 50) buckets[3].Count++;
            else buckets[4].Count++;
        }

        var vm = new DashboardViewModel
        {
            TotalPredictions = totalPredictions,
            PaymentTypeStats = paymentTypeStats,
            PriceBuckets = buckets,
            FromDate = fromDate,
            ToDate = toDate
        };

        return View(vm);
    }

}
