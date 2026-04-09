namespace AffiliateBackend.DTOs;

public class ArticleDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Intro { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<ArticleProductDto> Products { get; set; } = new();
    public List<ArticleTranslationDto> Translations { get; set; } = new();
}
