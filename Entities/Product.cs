namespace AffiliateBackend.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Asin { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string AffiliateUrl { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? Rating { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ArticleProduct> ArticleProducts { get; set; } = new();
}