using AffiliateBackend.Data;
using AffiliateBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace AffiliateBackend.Endpoints;

public static class ClickEndpoints
{
    public static IEndpointRouteBuilder MapClickEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/r/{articleId:guid}/{productId:guid}", async (
            AppDbContext db,
            Guid articleId,
            Guid productId,
            HttpContext http) =>
        {
            var articleExists = await db.Articles.AnyAsync(x => x.Id == articleId);
            if (!articleExists)
            {
                return Results.NotFound(new { message = "Article not found" });
            }

            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == productId && x.IsActive);
            if (product is null)
            {
                return Results.NotFound(new { message = "Product not found" });
            }

            db.OutboundClicks.Add(new OutboundClick
            {
                ArticleId = articleId,
                ProductId = productId,
                Referrer = http.Request.Headers.Referer.ToString(),
                UserAgent = http.Request.Headers.UserAgent.ToString(),
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();

            return Results.Redirect(product.AffiliateUrl);
        });

        app.MapGet("/api/clicks", async (AppDbContext db) =>
        {
            var clicks = await db.OutboundClicks
                .OrderByDescending(x => x.CreatedAt)
                .Take(50)
                .ToListAsync();

            return Results.Ok(clicks);
        });

        app.MapGet("/api/stats", async (AppDbContext db) =>
        {
            var totalClicks = await db.OutboundClicks.CountAsync();

            var clicksByProduct = await db.OutboundClicks
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Clicks = g.Count()
                })
                .OrderByDescending(x => x.Clicks)
                .Take(10)
                .ToListAsync();

            return Results.Ok(new
            {
                totalClicks,
                topProducts = clicksByProduct
            });
        });

        return app;
    }
}
