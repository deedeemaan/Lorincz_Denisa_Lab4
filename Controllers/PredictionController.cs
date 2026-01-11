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
}
