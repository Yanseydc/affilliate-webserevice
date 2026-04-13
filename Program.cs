using AffiliateBackend.Data;
using AffiliateBackend.Endpoints;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var postgresConnectionString = BuildPostgresConnectionString(
    Environment.GetEnvironmentVariable("PGHOST"),
    Environment.GetEnvironmentVariable("PGPORT"),
    Environment.GetEnvironmentVariable("PGDATABASE"),
    Environment.GetEnvironmentVariable("PGUSER"),
    Environment.GetEnvironmentVariable("PGPASSWORD"),
    databaseUrl);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(postgresConnectionString))
    {
        options.UseNpgsql(postgresConnectionString);
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

builder.Services.AddCors(options =>
{
    var frontendOrigins = (Environment.GetEnvironmentVariable("FRONTEND_ORIGINS") ??
                           "http://localhost:3000,https://affiliate-app-dun.vercel.app")
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

app.UseHttpsRedirection();

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database migration failed during startup.");
    throw;
}

app.MapGet("/", () => Results.Ok(new { message = "AffiliateBackend running" }));
app.MapProductEndpoints();
app.MapArticleEndpoints();
app.MapClickEndpoints();

app.Run();

static string BuildConnectionStringFromDatabaseUrl(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);

    var csb = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
        Database = uri.AbsolutePath.Trim('/'),
        Pooling = true
    };

    if (uri.Port > 0)
    {
        csb.Port = uri.Port;
    }

    return csb.ConnectionString;
}

static string? BuildPostgresConnectionString(
    string? pgHost,
    string? pgPort,
    string? pgDatabase,
    string? pgUser,
    string? pgPassword,
    string? databaseUrl)
{
    if (!string.IsNullOrWhiteSpace(pgHost) &&
        !string.IsNullOrWhiteSpace(pgPort) &&
        !string.IsNullOrWhiteSpace(pgDatabase) &&
        !string.IsNullOrWhiteSpace(pgUser) &&
        !string.IsNullOrWhiteSpace(pgPassword))
    {
        return $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};Pooling=true;SSL Mode=Disable";
    }

    return string.IsNullOrWhiteSpace(databaseUrl)
        ? null
        : BuildConnectionStringFromDatabaseUrl(databaseUrl);
}
