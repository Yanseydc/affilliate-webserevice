namespace AffiliateBackend.Entities;

public class OutboundClick
{
    public long Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ArticleId { get; set; }
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
}