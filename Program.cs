using AffiliateBackend.Data;
using AffiliateBackend.Endpoints;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

var pgHost = Environment.GetEnvironmentVariable("PGHOST");
var pgPort = Environment.GetEnvironmentVariable("PGPORT");
var pgDatabase = Environment.GetEnvironmentVariable("PGDATABASE");
var pgUser = Environment.GetEnvironmentVariable("PGUSER");
var pgPassword = Environment.GetEnvironmentVariable("PGPASSWORD");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(pgHost) &&
        !string.IsNullOrWhiteSpace(pgPort) &&
        !string.IsNullOrWhiteSpace(pgDatabase) &&
        !string.IsNullOrWhiteSpace(pgUser) &&
        !string.IsNullOrWhiteSpace(pgPassword))
    {
        var connectionString =
            $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};Pooling=true;SSL Mode=Disable";

        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000",
                "https://affiliate-app-dun.vercel.app"
            )
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

app.MapGet("/", () => Results.Ok(new { message = "AffiliateBackend running" }));
app.MapProductEndpoints();
app.MapArticleEndpoints();
app.MapClickEndpoints();

// try
// {
//     using var scope = app.Services.CreateScope();
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }
// catch (Exception ex)
// {
//     Console.WriteLine("Startup migration error:");
//     Console.WriteLine(ex.ToString());
//     throw;
// }

app.Run();

static string BuildConnectionStringFromDatabaseUrl(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);

    var csb = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = userInfo[0],
        Password = userInfo.Length > 1 ? userInfo[1] : "",
        Database = uri.AbsolutePath.Trim('/'),
        Pooling = true
    };

    return csb.ConnectionString;
}