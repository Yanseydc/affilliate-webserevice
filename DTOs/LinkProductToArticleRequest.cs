namespace AffiliateBackend.DTOs;

public class LinkProductToArticleRequest
{
    public Guid ProductId { get; set; }
    public int Position { get; set; }
    public string? CustomSummary { get; set; }
}
