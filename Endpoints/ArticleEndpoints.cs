using AffiliateBackend.Data;
using AffiliateBackend.DTOs;
using AffiliateBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace AffiliateBackend.Endpoints;

public static class ArticleEndpoints
{
    private static readonly string[] AllowedLanguages = ["en", "es"];

    public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/articles", async (AppDbContext db, CreateArticleRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { message = "Title is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                return Results.BadRequest(new { message = "Slug is required" });
            }

            var language = string.IsNullOrWhiteSpace(request.Language)
                ? "en"
                : request.Language.Trim().ToLowerInvariant();

            if (!AllowedLanguages.Contains(language))
            {
                return Results.BadRequest(new { message = "Language must be 'en' or 'es'" });
            }

            var slugExists = await db.Articles.AnyAsync(x => x.Slug == request.Slug);
            if (slugExists)
            {
                return Results.BadRequest(new { message = "Slug already exists" });
            }

            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = request.Slug,
                Intro = request.Intro,
                Status = request.Status,
                Language = language,
                TranslationGroupId = request.TranslationGroupId ??  Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            db.Articles.Add(article);
            await db.SaveChangesAsync();

            return Results.Created($"/api/articles/{article.Slug}", article);
        });
        
        app.MapGet("/api/articles", async (AppDbContext db) =>
        {
            var articles = await db.Articles
                .Where(x => x.Status == "published")
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Slug,
                    x.Language
                })
                .ToListAsync();

            return Results.Ok(articles);
        });

        app.MapGet("/api/articles/{slug}", async (AppDbContext db, string slug) =>
        {
            var article = await db.Articles
                .Include(x => x.ArticleProducts)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Slug == slug && x.Status == "published");

            if (article is null)
            {
                return Results.NotFound(new { message = "Article not found" });
            }

            var translations = new List<ArticleTranslationDto>();

            if (article.TranslationGroupId is not null)
            {
                translations = await db.Articles
                    .Where(x => x.TranslationGroupId == article.TranslationGroupId && x.Id != article.Id)
                    .Select(x => new ArticleTranslationDto
                    {
                        Language = x.Language,
                        Slug = x.Slug
                    })
                    .ToListAsync();
            }

            var response = new ArticleDetailResponse
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Intro = article.Intro,
                Language = article.Language,
                Products = article.ArticleProducts
                    .OrderBy(x => x.Position)
                    .Select(x => new ArticleProductDto
                    {
                        Id = x.Product.Id,
                        Title = x.Product.Title,
                        ImageUrl = x.Product.ImageUrl,
                        AffiliateUrl = x.Product.AffiliateUrl,
                        Price = x.Product.Price,
                        Rating = x.Product.Rating,
                        Position = x.Position,
                        CustomSummary = x.CustomSummary
                    })
                    .ToList(),
                Translations = translations
            };

            return Results.Ok(response);
        });

        app.MapGet("/api/articles/{id:guid}/translations", async (AppDbContext db, Guid id) =>
        {
            var article = await db.Articles.FirstOrDefaultAsync(x => x.Id == id);

            if (article is null)
            {
                return Results.NotFound(new { message = "Article not found" });
            }

            if (article.TranslationGroupId is null)
            {
                return Results.Ok(Array.Empty<Article>());
            }

            var translations = await db.Articles
                .Where(x => x.TranslationGroupId == article.TranslationGroupId && x.Id != article.Id)
                .OrderBy(x => x.Language)
                .ToListAsync();

            return Results.Ok(translations);
        });

        app.MapPost("/api/articles/{articleId:guid}/products", async (
            AppDbContext db,
            Guid articleId,
            LinkProductToArticleRequest request) =>
        {
            if (request.ProductId == Guid.Empty)
            {
                return Results.BadRequest(new { message = "ProductId is required" });
            }

            if (request.Position < 1)
            {
                return Results.BadRequest(new { message = "Position must be greater than or equal to 1" });
            }

            var articleExists = await db.Articles.AnyAsync(x => x.Id == articleId);
            if (!articleExists)
            {
                return Results.NotFound(new { message = "Article not found" });
            }

            var productExists = await db.Products.AnyAsync(x => x.Id == request.ProductId);
            if (!productExists)
            {
                return Results.NotFound(new { message = "Product not found" });
            }

            var relationExists = await db.ArticleProducts.AnyAsync(x =>
                x.ArticleId == articleId && x.ProductId == request.ProductId);

            if (relationExists)
            {
                return Results.BadRequest(new { message = "Product already linked to article" });
            }

            var articleProduct = new ArticleProduct
            {
                ArticleId = articleId,
                ProductId = request.ProductId,
                Position = request.Position,
                CustomSummary = request.CustomSummary
            };

            db.ArticleProducts.Add(articleProduct);
            await db.SaveChangesAsync();

            return Results.Ok(articleProduct);
        });

        return app;
    }
}
