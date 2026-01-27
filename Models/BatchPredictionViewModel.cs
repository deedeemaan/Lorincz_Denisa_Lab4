using static Lorincz_Denisa_Lab4.PricePredictionModel;

public class BatchPredictionViewModel
{
    public IFormFile? File { get; set; }
    public List<ModelOutput>? Predictions { get; set; }
    public string? ErrorMessage { get; set; }
}
