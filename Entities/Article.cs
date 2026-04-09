namespace AffiliateBackend.Entities;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Intro { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Language { get; set; } = "en";
    public Guid? TranslationGroupId { get; set; }
    public List<ArticleProduct> ArticleProducts { get; set; } = new();
}