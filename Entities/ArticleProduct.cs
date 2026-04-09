namespace AffiliateBackend.Entities;

public class ArticleProduct
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Position { get; set; }
    public string? CustomSummary { get; set; }
}