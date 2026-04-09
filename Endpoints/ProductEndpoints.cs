using AffiliateBackend.Data;
using AffiliateBackend.DTOs;
using AffiliateBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace AffiliateBackend.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/products", async (AppDbContext db, CreateProductRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { message = "Title is required" });
            }

            if (string.IsNullOrWhiteSpace(request.AffiliateUrl))
            {
                return Results.BadRequest(new { message = "AffiliateUrl is required" });
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Asin = request.Asin,
                Title = request.Title,
                ImageUrl = request.ImageUrl,
                AffiliateUrl = request.AffiliateUrl,
                Price = request.Price,
                Rating = request.Rating,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            return Results.Created($"/api/products/{product.Id}", product);
        });

        app.MapGet("/api/products", async (AppDbContext db) =>
        {
            var products = await db.Products
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Results.Ok(products);
        });

        return app;
    }
}
