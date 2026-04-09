using AffiliateBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace AffiliateBackend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleProduct> ArticleProducts => Set<ArticleProduct>();
    public DbSet<OutboundClick> OutboundClicks => Set<OutboundClick>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleProduct>()
            .HasKey(x => new { x.ArticleId, x.ProductId });

        modelBuilder.Entity<ArticleProduct>()
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleProducts)
            .HasForeignKey(x => x.ArticleId);

        modelBuilder.Entity<ArticleProduct>()
            .HasOne(x => x.Product)
            .WithMany(x => x.ArticleProducts)
            .HasForeignKey(x => x.ProductId);

        modelBuilder.Entity<Article>()
            .HasIndex(x => x.Slug)
            .IsUnique();
        
        modelBuilder.Entity<Article>()
            .HasIndex(x => x.TranslationGroupId);
        
        modelBuilder.Entity<Article>()
            .HasIndex(x => x.Language);
    }
}