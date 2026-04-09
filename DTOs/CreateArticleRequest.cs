namespace AffiliateBackend.DTOs;

public class CreateArticleRequest
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Intro { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public string Language { get; set; } = "en";
    public Guid? TranslationGroupId { get; set; }
}
