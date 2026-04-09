namespace AffiliateBackend.DTOs;

public class ArticleProductDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string AffiliateUrl { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? Rating { get; set; }
    public int Position { get; set; }
    public string? CustomSummary { get; set; }
}
