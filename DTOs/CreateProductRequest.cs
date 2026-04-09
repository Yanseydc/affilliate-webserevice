namespace AffiliateBackend.DTOs;

public class CreateProductRequest
{
    public string Asin { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string AffiliateUrl { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? Rating { get; set; }
    public bool IsActive { get; set; } = true;
}
